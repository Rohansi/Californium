using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace Californium
{
    public class EntityManager : Drawable
    {
        private const float CleanupEvery = 60.0f;

        public ReadOnlyCollection<Entity> Entities
        {
            get { return entities.AsReadOnly(); }
        }

        private List<Entity> entities;
        private List<Entity> inputEntities; 

        private Dictionary<Vector2i, List<Entity>> entityGrid;
        private float cleanupTimer;

        private State parent;

        private Entity currentEntity;

        public EntityManager(State parent)
        {
            entities = new List<Entity>();
            inputEntities = new List<Entity>();
            entityGrid = new Dictionary<Vector2i, List<Entity>>();

            this.parent = parent;
        }

        public bool ProcessInput(InputArgs args)
        {
            return inputEntities.Any(e => e.Input.ProcessInput(args));
        }

        public void Update(float dt)
        {
            var readonlyEntities = new List<Entity>(entities);
            var newGridPos = new Vector2i();

            foreach (var e in readonlyEntities)
            {
                currentEntity = e;
                e.Update(dt);
                currentEntity = null;

                newGridPos.X = (int)e.Position.X / GameOptions.EntityGridSize;
                newGridPos.Y = (int)e.Position.Y / GameOptions.EntityGridSize;

                if (!e.GridCoordinate.Equals(newGridPos))
                {
                    GridRemove(e);
                    e.GridCoordinate = newGridPos;
                    GridAdd(e);
                }
            }

            cleanupTimer += dt;

            if (cleanupTimer >= CleanupEvery)
            {
                entityGrid.RemoveAll(kv => kv.Value.Count == 0);
                cleanupTimer = 0;
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            View view = target.GetView();

            FloatRect screenBounds = new FloatRect
            {
                Left = view.Center.X - (view.Size.X / 2),
                Top = view.Center.Y - (view.Size.Y / 2),
                Width = view.Size.X,
                Height = view.Size.Y
            };

            foreach (var e in InArea(screenBounds))
            {
                e.Draw(target);
            }
        }

        public void Add(Entity e)
        {
            entities.Add(e);

            e.Parent = parent;
            e.GridCoordinate = new Vector2i((int)e.Position.X / GameOptions.EntityGridSize, (int)e.Position.Y / GameOptions.EntityGridSize);

            GridAdd(e);

            if (e.Input != null)
                inputEntities.Add(e);
        }

        public void Remove(Entity e)
        {
            GridRemove(e);
            entities.Remove(e);

            if (e.Input != null)
                inputEntities.Remove(e);
        }

        public void RemoveAll(Func<Entity, bool> match)
        {
            foreach (var e in entities.Where(match).ToList())
            {
                Remove(e);
            }
        }

        public Entity Find(Predicate<Entity> match)
        {
            return entities.Find(match);
        }

        public List<Entity> FindAll(Predicate<Entity> match)
        {
            return entities.FindAll(match);
        }

        public IEnumerable<Entity> InArea(FloatRect rect)
        {
            int startX = ((int)rect.Left / GameOptions.EntityGridSize);
            int startY = ((int)rect.Top / GameOptions.EntityGridSize);
            int width = ((int)rect.Width / GameOptions.EntityGridSize) + 1;
            int height = ((int)rect.Height / GameOptions.EntityGridSize) + 1;

            var pos = new Vector2i();

            for (int y = startY; y < startY + height; y++)
            {
                for (int x = startX; x < startX + width; x++)
                {
                    pos.X = x;
                    pos.Y = y;

                    List<Entity> list;
                    if (entityGrid.TryGetValue(pos, out list))
                    {
                        foreach (Entity e in list)
                        {
                            yield return e;
                        }
                    }
                }
            }
        }

        public bool PlaceFree(FloatRect r, float overScan = 64f)
        {
            var region = new FloatRect(r.Left - overScan, r.Top - overScan, r.Width + (overScan * 2), r.Height + (overScan * 2));

            return InArea(region).All(entity => entity == currentEntity || !entity.Solid || !entity.BoundingBox.Intersects(r));
        }

        internal void AddInput(Entity e)
        {
            inputEntities.Add(e);
        }

        private void GridAdd(Entity e)
        {
            List<Entity> list;

            if (!entityGrid.TryGetValue(e.GridCoordinate, out list))
            {
                list = new List<Entity> { e };
                entityGrid.Add(e.GridCoordinate, list);
                return;
            }

            list.Add(e);
        }

        private void GridRemove(Entity e)
        {
            List<Entity> list;

            if (entityGrid.TryGetValue(e.GridCoordinate, out list))
                list.Remove(e);
        }
    }
}

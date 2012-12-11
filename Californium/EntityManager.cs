using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.Window;

namespace Californium
{
    public class EntityManager : Drawable
    {
        private const float CleanupEvery = 60.0f;

        private LinkedList<Entity> entities;
        private List<Entity> inputEntities;

        private Dictionary<Vector2i, LinkedList<Entity>> entityGrid;
        private float cleanupTimer;

        private State parent;

        private Entity currentEntity;

        public EntityManager(State parent)
        {
            entities = new LinkedList<Entity>();
            inputEntities = new List<Entity>();
            entityGrid = new Dictionary<Vector2i, LinkedList<Entity>>();

            this.parent = parent;
        }

        public bool ProcessInput(InputArgs args)
        {
            return inputEntities.Any(e => e.Input.ProcessInput(args));
        }

        public void Update()
        {
            var newGridPos = new Vector2i();

            var cur = entities.First;
            while (cur != null)
            {
                var next = cur.Next;
                var e = cur.Value;

                currentEntity = e;
                e.Update();
                currentEntity = null;

                newGridPos.X = (int)e.Position.X / GameOptions.EntityGridSize;
                newGridPos.Y = (int)e.Position.Y / GameOptions.EntityGridSize;

                if (!e.GridCoordinate.Equals(newGridPos))
                {
                    GridRemove(e);
                    e.GridCoordinate = newGridPos;
                    GridAdd(e);
                }

                cur = next;
            }

            cleanupTimer += GameOptions.Timestep;

            if (cleanupTimer >= CleanupEvery)
            {
                entityGrid.RemoveAll(kv => kv.Value.Count == 0);
                cleanupTimer = 0;
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            var view = target.GetView();
            var screenBounds = new FloatRect
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
            e.Parent = parent;
            e.Node = entities.AddLast(e);
            e.GridCoordinate = new Vector2i((int)e.Position.X / GameOptions.EntityGridSize, (int)e.Position.Y / GameOptions.EntityGridSize);

            GridAdd(e);

            if (e.Input != null)
                inputEntities.Add(e);
        }

        public void Remove(Entity e)
        {
            GridRemove(e);
            entities.Remove(e.Node);

            if (e.Input != null)
                inputEntities.Remove(e);
        }

        public IEnumerable<Entity> InArea(FloatRect rect)
        {
            var overscan = GameOptions.EntityOverscan;
            var gridSize = GameOptions.EntityGridSize;

            rect = new FloatRect(rect.Left - overscan, rect.Top - overscan,
                                 rect.Width + (overscan * 2), rect.Height + (overscan * 2));

            var startX = (int)rect.Left / gridSize;
            var startY = (int)rect.Top / gridSize;
            var width = (int)rect.Width / gridSize + 1;
            var height = (int)rect.Height / gridSize + 1;

            var pos = new Vector2i();

            for (var y = startY; y < startY + height; y++)
            {
                for (var x = startX; x < startX + width; x++)
                {
                    pos.X = x;
                    pos.Y = y;

                    LinkedList<Entity> list;
                    if (entityGrid.TryGetValue(pos, out list))
                    {
                        var cur = list.First;
                        while (cur != null)
                        {
                            var next = cur.Next;
                            yield return cur.Value;
                            cur = next;
                        }
                    }
                }
            }
        }

        public bool PlaceFree(FloatRect rect)
        {
            return InArea(rect).All(entity => entity == currentEntity || !entity.Solid || !entity.BoundingBox.Intersects(rect));
        }

        internal void AddInput(Entity e)
        {
            inputEntities.Add(e);
        }

        private void GridAdd(Entity e)
        {
            LinkedList<Entity> list;

            if (!entityGrid.TryGetValue(e.GridCoordinate, out list))
            {
                list = new LinkedList<Entity>();
                list.AddLast(e);
                entityGrid.Add(e.GridCoordinate, list);
                return;
            }

            list.AddLast(e);
        }

        private void GridRemove(Entity e)
        {
            LinkedList<Entity> list;

            if (entityGrid.TryGetValue(e.GridCoordinate, out list))
                list.Remove(e);
        }
    }
}

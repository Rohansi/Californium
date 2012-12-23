using System;

namespace Californium
{
    public delegate double TweenFunc(double? overrideTimer = null);
    public delegate void TweenCallback();

    public enum TweenType
    {
        InQuad, OutQuad, InOutQuad, InCubic, OutCubic, InOutCubic, InQuart, OutQuart, InOutQuart,
        InQuint, OutQuint, InOutQuint, InSine, OutSine, InOutSine, InExpo, OutExpo, InOutExpo,
        InCirc, OutCirc, InOutCirc, InElastic, OutElastic, InOutElastic, InBack, OutBack,
        InOutBack, InBounce, OutBounce, InOutBounce, Linear
    }

    public static class Tween
    {
        public static double BackAmount = 1.70158;

        public static TweenFunc Create(TweenType tweenType, double startValue, double endValue, double duration, TweenCallback finished = null)
        {
            var time = 0.0;
            var called = false;

            return overrideTimer =>
            {
                var t = overrideTimer != null ? overrideTimer.Value : (time = Math.Min(time + GameOptions.Timestep, duration));

                if (t >= duration && !called && finished != null)
                {
                    called = true;
                    finished();
                }

                return (TweenMain(tweenType, t, duration) / (1 / (endValue - startValue))) + startValue;
            };
        }

        public static TweenFunc None(double value, double duration = 0, TweenCallback finished = null)
        {
            var time = 0.0;
            var called = false;

            return overrideTimer =>
            {
                var t = overrideTimer != null ? overrideTimer.Value : (time = Math.Min(time + GameOptions.Timestep, duration));

                if (t >= duration && !called && finished != null)
                {
                    called = true;
                    finished();
                }

                return value;
            };
        }

        private static double TweenMain(TweenType tweenType, double t, double d, double b = 0, double c = 1)
        {
            var backS = BackAmount;

            switch (tweenType)
            {
                case TweenType.Linear:
                    return c * t / d + b;

                case TweenType.InQuad:
                    return c * (t /= d) * t + b;

                case TweenType.OutQuad:
                    return -c * (t /= d) * (t - 2) + b;

                case TweenType.InOutQuad:
                    if ((t /= d / 2) < 1)
                        return c / 2 * t * t + b;
                    return -c / 2 * ((--t) * (t - 2) - 1) + b;

                case TweenType.InCubic:
                    return c * (t /= d) * t * t + b;

                case TweenType.OutCubic:
                    return c * ((t = t / d - 1) * t * t + 1) + b;

                case TweenType.InOutCubic:
                    if ((t /= d / 2) < 1)
                        return c / 2 * t * t * t + b;
                    return c / 2 * ((t -= 2) * t * t + 2) + b;

                case TweenType.InQuart:
                    return c * (t /= d) * t * t * t + b;

                case TweenType.OutQuart:
                    return -c * ((t = t / d - 1) * t * t * t - 1) + b;

                case TweenType.InOutQuart:
                    if ((t /= d / 2) < 1)
                        return c / 2 * t * t * t * t + b;
                    return -c / 2 * ((t -= 2) * t * t * t - 2) + b;

                case TweenType.InQuint:
                    return c * (t /= d) * t * t * t * t + b;

                case TweenType.OutQuint:
                    return c * ((t = t / d - 1) * t * t * t * t + 1) + b;

                case TweenType.InOutQuint:
                    if ((t /= d / 2) < 1)
                        return c / 2 * t * t * t * t * t + b;
                    return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;

                case TweenType.InSine:
                    return -c * Math.Cos(t / d * (Math.PI / 2)) + c + b;

                case TweenType.OutSine:
                    return c * Math.Sin(t / d * (Math.PI / 2)) + b;

                case TweenType.InOutSine:
                    return -c / 2 * (Math.Cos(Math.PI * t / d) - 1) + b;

                case TweenType.InExpo:
                    return (t <= 0) ? b : c * Math.Pow(2, 10 * (t / d - 1)) + b;

                case TweenType.OutExpo:
                    return (t >= d) ? b + c : c * (-Math.Pow(2, -10 * t / d) + 1) + b;

                case TweenType.InOutExpo:
                    if (t <= 0)
                        return b;
                    if (t >= d)
                        return b + c;
                    if ((t /= d / 2) < 1)
                        return c / 2 * Math.Pow(2, 10 * (t - 1)) + b;
                    return c / 2 * (-Math.Pow(2, -10 * --t) + 2) + b;

                case TweenType.InCirc:
                    return -c * (Math.Sqrt(1 - (t /= d) * t) - 1) + b;

                case TweenType.OutCirc:
                    return c * Math.Sqrt(1 - (t = t / d - 1) * t) + b;

                case TweenType.InOutCirc:
                    if ((t /= d / 2) < 1)
                        return -c / 2 * (Math.Sqrt(1 - t * t) - 1) + b;
                    return c / 2 * (Math.Sqrt(1 - (t -= 2) * t) + 1) + b;

                case TweenType.InElastic:
                    {
                        double s;
                        var p = d * 0.3;
                        var a = c;

                        if (t <= 0)
                            return b;
                        if ((t /= d) >= 1)
                            return b + c;
                        if (a < Math.Abs(c))
                        {
                            a = c;
                            s = p / 4;
                        }
                        else
                            s = p / (2 * Math.PI) * Math.Asin(c / a);

                        return -(a * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b;
                    }

                case TweenType.OutElastic:
                    {
                        double s;
                        var p = d * .3;
                        var a = c;

                        if (t <= 0)
                            return b;
                        if ((t /= d) >= 1)
                            return b + c;
                        if (a < Math.Abs(c))
                        {
                            a = c;
                            s = p / 4;
                        }
                        else
                            s = p / (2 * Math.PI) * Math.Asin(c / a);

                        return a * Math.Pow(2, -10 * t) * Math.Sin((t * d - s) * (2 * Math.PI) / p) + c + b;
                    }

                case TweenType.InOutElastic:
                    {
                        double s;
                        var p = d * (.3 * 1.5);
                        var a = c;

                        if (t <= 0)
                            return b;
                        if ((t /= d / 2) >= 2)
                            return b + c;
                        if (a < Math.Abs(c))
                        {
                            a = c;
                            s = p / 4;
                        }
                        else
                            s = p / (2 * Math.PI) * Math.Asin(c / a);

                        if (t < 1)
                            return -.5 * (a * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b;
                        return a * Math.Pow(2, -10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p) * .5 + c + b;
                    }

                case TweenType.InBounce:
                    {
                        var outBounce = Create(TweenType.OutBounce, 0, c, d);
                        return c - outBounce(d - t) + b;
                    }

                case TweenType.OutBounce:
                    if ((t /= d) < (1 / 2.75))
                        return c * (7.5625 * t * t) + b;
                    if (t < (2 / 2.75))
                        return c * (7.5625 * (t -= (1.5 / 2.75)) * t + .75) + b;
                    if (t < (2.5 / 2.75))
                        return c * (7.5625 * (t -= (2.25 / 2.75)) * t + .9375) + b;
                    return c * (7.5625 * (t -= (2.625 / 2.75)) * t + .984375) + b;

                case TweenType.InOutBounce:
                    {
                        var inBounce = Create(TweenType.InBounce, 0, c, d);
                        var outBounce = Create(TweenType.OutBounce, 0, c, d);

                        if (t < d / 2)
                            return inBounce(t * 2) * .5 + b;
                        return outBounce(t * 2 - d) * .5 + c * .5 + b;
                    }

                case TweenType.InBack:
                    return c * (t /= d) * t * ((backS + 1) * t - backS) + b;

                case TweenType.OutBack:
                    return c * ((t = t / d - 1) * t * ((backS + 1) * t + backS) + 1) + b;

                case TweenType.InOutBack:
                    if ((t /= d / 2) < 1)
                        return c / 2 * (t * t * (((backS *= (1.525)) + 1) * t - backS)) + b;
                    return c / 2 * ((t -= 2) * t * (((backS *= (1.525)) + 1) * t + backS) + 2) + b;

                default:
                    return b;
            }
        }
    }
}

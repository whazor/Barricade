namespace Barricade.Utilities
{
    public sealed class CountedRandom : System.Random
    {
        public int Seed { get; private set; }
        public int Counter { get; private set; }

        public CountedRandom(int seed, int counter)
            : base(seed)
        {
            Seed = seed;
            for (int i = 0; i < counter; i++)
            {
                Next();
            }
        }

        public override int Next()
        {
            Counter++;
            return base.Next();
        }
        public override int Next(int a)
        {
            Counter++;
            return base.Next(a);
        }
        public override int Next(int a, int b)
        {
            Counter++;
            return base.Next(a, b);
        }
        public override void NextBytes(byte[] buffer)
        {
            Counter++;
            base.NextBytes(buffer);
        }
        public override double NextDouble()
        {
            Counter++;
            return base.NextDouble();
        }
        protected override double Sample()
        {
            Counter++;
            return base.Sample();
        }
    }
}
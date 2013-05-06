namespace Morpheus
{
    public class Product
    {
        public ProductType ProductType { get; private set; }

        public int Number { get; private set; }

        public double Mass { get; private set; }

        public Product(ProductType productType, int number, double mass)
        {
            ProductType = productType;
            Number = number;
            Mass = mass;
        }

        public override string ToString()
        {
            return ProductType.ToString() + Number.ToString();
        }
    }
}
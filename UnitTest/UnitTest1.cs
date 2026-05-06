using Xunit;

namespace UnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            string something = "123-765";
            string[] ids = something.Split('-');

            string idOne = ids[0];
            string idTwo = ids[1];

            Console.WriteLine(idOne, idTwo);
        }
    }
}
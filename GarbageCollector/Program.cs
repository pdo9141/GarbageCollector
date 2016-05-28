using System;
using System.IO;

namespace GarbageCollector
{
    class Program
    {
        private static long B = 1, KB = 1024, MB = KB * 1024, GB = MB * 1024, TB = GB * 1024;

        static void Main(string[] args)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            TestLargeObjectHeap();
            TestReadLargeFileInGC();
            TestWeakReference();
            TestGenerationOfObject();
        }

        private static void TestLargeObjectHeap()
        {
            long bytes = GC.GetTotalMemory(false);
            Console.WriteLine("TotalMemory: {0}", bytes);

            string a = File.ReadAllText(@"C:\temp\Test.txt");
            Console.WriteLine(GC.GetGeneration(a));

            long bytes1 = GC.GetTotalMemory(false);
            Console.WriteLine("TotalMemory: {0}", bytes1);
            Console.WriteLine("TotalMemory Difference: {0}", bytes1 - bytes);
            
            GC.Collect();
            Console.WriteLine(GC.GetGeneration(a));

            long bytes2 = GC.GetTotalMemory(false);
            Console.WriteLine("TotalMemory: {0}", bytes2);
            Console.WriteLine("TotalMemory Difference: {0}", bytes2 - bytes1);

            string b = File.ReadAllText(@"C:\temp\TestLarge.txt");
            Console.WriteLine(GC.GetGeneration(b));

            long bytes3 = GC.GetTotalMemory(false);
            Console.WriteLine("TotalMemory: {0}", bytes3);
            Console.WriteLine("TotalMemory Difference: {0}", bytes3 - bytes2);

            GC.Collect();
            Console.WriteLine(GC.GetGeneration(b));

            long bytes4 = GC.GetTotalMemory(false);
            Console.WriteLine("TotalMemory: {0}", bytes4);
            Console.WriteLine("TotalMemory Difference: {0}", bytes4 - bytes3);

            Obj o = new Obj();
            o.Allocate(85000);
            Console.WriteLine(GC.GetGeneration(o));
            Console.WriteLine(GC.GetGeneration(o.items));
            Console.WriteLine(GC.GetGeneration(o.items2));
            Console.WriteLine(GC.GetGeneration(o.Data));
            Console.ReadLine();
        }

        private static void TestReadLargeFileInGC()
        {
            long bytes = GC.GetTotalMemory(false);
            Console.WriteLine("TotalMemory: {0}", bytes);

            CreateDog(bytes, "Kirby", 15, MB);

            bytes = GC.GetTotalMemory(false);
            CreateDog(bytes, "Oscar", 27, MB);

            bytes = GC.GetTotalMemory(false);
            CreateDog(bytes, "Winston", 9, MB);

            GC.Collect();
            bytes = GC.GetTotalMemory(false);
            Console.WriteLine("TotalMemory: {0}", bytes);
            Console.ReadLine();
        }

        private static void TestWeakReference()
        {
            Dog dog = new Dog("Bowser", 5, MB);

            WeakReference dogRef = new WeakReference(dog);
            Console.WriteLine(dogRef.IsAlive);

            dog = null;
            GC.Collect();

            Console.WriteLine(dogRef.IsAlive);
            Console.ReadLine();
        }

        private static void CreateDog(long bytes, string name, int age, long size)
        {
            Dog dog1 = new Dog(name, age, size);
            dog1.Bark();

            long bytes1 = GC.GetTotalMemory(false);
            Console.WriteLine("TotalMemory: {0}", bytes1);
            Console.WriteLine("TotalMemory Difference: {0}", bytes1 - bytes);
        }

        private static void TestGenerationOfObject()
        {
            Dog bob = new Dog("Bob", 5, MB);
            Console.WriteLine(string.Format("Bob is in generation {0}", GC.GetGeneration(bob)));

            GC.Collect();
            Console.WriteLine(string.Format("Bob is in generation {0}", GC.GetGeneration(bob)));

            GC.Collect();
            Console.WriteLine(string.Format("Bob is in generation {0}", GC.GetGeneration(bob)));
            Console.ReadLine();
        }
    }
    

    public class Dog
    {
        private byte[] _data = null;

        public long Size { get; set; } 

        public string Name { get; set; }

        public int Age { get; set; }

        public Dog()
        {
        }

        public Dog(string name, int age, long size)
        {
            Name = name;
            Age = age;
            //_data = new byte[size];
            _data = File.ReadAllBytes(@"C:\temp\TestLarge.doc");
        }

        public void Bark()
        {
            Console.WriteLine("{0} is barking", Name);
            Console.WriteLine("");
        }
    }

    public class Obj
    {
        public byte[] items = null;

        public byte[] items2 = null;

        public string Data = string.Empty;

        public void Allocate(int i)
        {
            items = new byte[i];
            items2 = new byte[10];
            Data = System.Text.Encoding.UTF8.GetString(items);
        }
    }
}

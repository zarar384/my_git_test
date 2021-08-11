using System;

namespace my_git_test{
    class Person{
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public Person(string name, string surname, int age)
        {
            Name = name;
            Surname = surname;
            Age = age;
        }
        public void Print(){
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("My name is {0}, my surname is {1} and I'm {2} years old", Name, Surname, Age);
            Console.ResetColor();
        }
    }
}
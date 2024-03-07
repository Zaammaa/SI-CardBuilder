using Spirit_Island_Card_Generator.Classes.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes
{
    internal class ReflectionManager
    {
        private static Dictionary<Type, List<Type>> foundTypes = new Dictionary<Type, List<Type>>();

        public static List<T> GetInstanciatedSubClasses<T>()
        {
            List<Type> types = GetSubClasses<T>();
            List<T> instanciated = new List<T>();
            foreach (Type type in types)
            {
                if (type.IsClass && !type.IsAbstract && type.GetConstructor(Type.EmptyTypes) != null)
                {
                    // Instantiate the class using Activator.CreateInstance
                    object instance = Activator.CreateInstance(type);

                    // If needed, cast the instance to the Effect class
                    T classInstance = (T)instance;

                    // Now you can use the instantiated object
                    // For example, you can call methods or set properties
                    instanciated.Add(classInstance);
                }
            }
            return instanciated;
        }

        public static List<Type> GetSubClasses<T>()
        {
            if (foundTypes.ContainsKey(typeof(T)))
            {
                return foundTypes[typeof(T)];
            }

            string directoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            List<Type> derivedTypes = new List<Type>();

            // Get all .dll and .exe files in the directory and its subdirectories
            string[] files = Directory.GetFiles(directoryPath, "*.dll", SearchOption.AllDirectories)
                .Concat(Directory.GetFiles(directoryPath, "*.exe", SearchOption.AllDirectories))
                .ToArray();

            // Iterate through each assembly file
            foreach (string file in files)
            {
                try
                {
                    // Load the assembly
                    Assembly assembly = Assembly.LoadFrom(file);

                    // Get all types in the assembly
                    Type[] types = assembly.GetTypes();

                    // Filter types that inherit from MyBaseClass
                    foreach (Type type in types)
                    {
                        if (type.IsSubclassOf(typeof(T)))
                        {
                            derivedTypes.Add(type);
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Console.WriteLine($"Error loading types from assembly {file}: {ex.Message}");
                    // Handle the error
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing assembly {file}: {ex.Message}");
                    // Handle the error
                }
            }

            foundTypes.Add(typeof(T), derivedTypes);
            return derivedTypes;

        }
    }
}

// ## 2.1 Using ## //
// Other Libraries
using Unity.Engine;
// Own Class
// using Other.OwnClass; 

// Adjust namespaces, according to Folder structure
// Curly Brackets are below the Code and not on the same Line
namespace DefaultNamespace
{
    public class Code_Convention
    {
        // ## 2.2 Class Variables ## //
        // private Variable beginns with a underscore
        // [SerializeField] only when needed 
        [SerializeField] private string _nameOfCar;
        
        // Readonly: Can be initialized at Compiletime or Runtime
        private readonly string PathToStoryFile;
        // Const: Can only be initialized at Compiletime
        private const int MaxSpeed = 120; 
        
        
        // ## 2.3 Methods ## //
        // Method Name starts with Verb and UpperCamelCase
        private string GetCarName()
        {
            return _nameOfCar;
        }
        
        // ## 2.4 Method Variables ## //
        public void ReadMiscellaneous()
        {
            // Variable without underscore
            int number = 0;
            // use of var, initialize it, so var knows what type will be stored
            var text = "";
            
            // Other situations to use var
            // foreach
            var userList = new List<string>();
            foreach (var item in userList)
            {
                // Do smth
            }
            
            // ## 2.5 Miscellaneous ## //
            // Simplified If
            if (number == 1)
                number = 12;
            
            // Simplified For
            for (int i = 0; i < 5; i++)
                number += i;
            // Simplified Foreach
            foreach (var item in userList)
                text += user + ";";
            // Simplifiedd While
            while (int i < number)
                text = GetCarName();
        }
    }
}
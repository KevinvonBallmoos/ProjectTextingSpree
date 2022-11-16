// Using
// Other Libraries
using Unity.Engine;
// Own Class
// using Other.OwnClass; 

// Adjust namespaces, according to Folder structure
namespace DefaultNamespace
{
    public class Code_Convention
    {
        // Class Variables
        // private Variable beginns with a underscore
        // [SerializeField] only when needed 
        [SerializeField] private string _nameOfCar;
        
        // Method Variables
        public void MethodVariables()
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
        }
    }
}
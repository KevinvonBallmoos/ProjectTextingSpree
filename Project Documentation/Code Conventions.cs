// ## 2.1 Using ## //
// Other Libraries
using Unity.Engine;
// Own Class
// using Other.OwnClass; 

// Logger Using
// using Code.Logger;

// Adjust namespaces, according to Folder structure
// Curly Brackets are below the Code and not on the same Line
namespace DefaultNamespace
{
    /// <summary>
    /// Class shows Code Convetions
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">24.11.2022</para>
    public class Code_Convention
    {
        // Logger Instantiate
        private readonly GameLogger _logger = new GameLogger("DialogueEditor");
        
        // ## 2.2 Class Variables ## //
        // private Variable beginns with a underscore
        private string _nameOfCar;
        // [SerializeField] only when needed  and no underscore
        [SerializeField] private string nameOfCar;
        
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
                text += user
                        sin+ ";";
            // Simplifiedd While
            while (int i < number)
                text = GetCarName();
                
            // Logger Needs Type (Test, Info, Exception, ...), Message and the Method _logger.GetLineNumber() returns the current Line Number
            _logger.LogEntry("Test", "Logging Info", _logger.GetLineNumber());
        }
        
        
        // ## 2.6 Comments ## //
        // #region <RegionName>, needs to be closed wiht the #endregion tag
        #region Comments
        
        // Lists
        private List<string> _list1;
        private List<string> _list2;
        private List<string> _list3;
        
        // Arrays 
        private int[] _numbers;
        private int[] _digits;
        private int[] _oddNumbers;

        /// <summary>
        /// Returns a number converted into a string
        /// </summary>
        /// <param name="index">provides the index of the Array</param>
        /// <returns>number as string</returns>
        private string GetNumberAsString(int index)
        {
            return _number[index];
        }
        
        // Getter for name Of Car
        private string GetCarName()
        {
            return _nameOfCar;
        }

        // Getter for string ID
        private string ID { get; set; }

        #endregion
        
        
        // Exception Handling
        private void ExceptionHandling()
        {
            try
            {
                // Some Code
            }
            catch (Exception ex)
            {
                // Alternative to ex.Message is ex.GetBaseException().ToString(), this gets the InnerException and is more detailed
                // The new StackTrace Line Number returns the excat line where the exception has been thrown
                _logger.LogEntry("Exception or Error", ex.Message, new StackTrace(ex, true).GetFrame(0).GetFileLineNumber());
            }

            using ()
            {
                //does automatically dispose of all, that is not used anymore
            }
        }
    }
}
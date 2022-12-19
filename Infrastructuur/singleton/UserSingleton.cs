using Infrastructuur.Database;
using Infrastructuur.Models;
using Infrastructuur.Services.Classes;
using Infrastructuur.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.singleton
{
    public sealed class UserSingleton : ControllerBase
    {
        /*A singleton is a design pattern that ensures that a class has only one instance and provides a global point of access to it. 
         * Here is an example of how you can implement a singleton in C#:*/
        private static UserSingleton instance = null;
        /*padlock object is used as a synchronization object to ensure thread safety when accessing the instance field.
       The padlock object is a private static readonly object that is used as a mutex (a mutual exclusion object) to synchronize access to the instance field. The padlock object is used in a lock statement, which acquires a mutual-exclusion lock on the object for the duration of the statement. This ensures that only one thread can execute the code inside the lock block at a time, preventing race conditions and ensuring that the instance field is accessed in a thread-safe manner.
       The use of the padlock object is important in the singleton pattern because it ensures that the instance field is initialized correctly even if multiple threads try to access it concurrently. Without the lock statement, it is possible that two threads could both execute the code to create the instance field, leading to multiple instances of the Singleton class being created.
       The padlock object is made readonly to ensure that it is initialized only once and to prevent it from being modified by other code. This further helps to ensure thread safety and prevent race conditions.*/
        private static readonly object padlock = new object();
        private UserEntity user;

        public UserSingleton() 
        {

        }
     

       

        public static UserSingleton Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new UserSingleton();
                    }
                    return instance;
                }
            }
        }

        public UserEntity User { get => user; set => user = value; }
    }
}

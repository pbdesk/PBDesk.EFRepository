using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PBDesk.EFRepository.Exceptions
{
    [Serializable]
    public class EFRepositoryException : Exception
    {
        public string EFMethodName { get; set; }

        public EFRepositoryException()
        {
        }

        public EFRepositoryException(string message, string methodName)
            : base("EFRepositoryException: " + message)
        {
            if (!string.IsNullOrWhiteSpace(methodName))
            {
                EFMethodName = methodName;
            }
            else
            {
                EFMethodName = string.Empty;
            }
        }
        public EFRepositoryException(string message, string methodName, Exception innerException)
            : base("EFRepositoryException: " + message, innerException)
        {
            if (!string.IsNullOrWhiteSpace(methodName))
            {
                EFMethodName = methodName;
            }
            else
            {
                EFMethodName = string.Empty;
            }
        }

        protected EFRepositoryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
            {
                this.EFMethodName = info.GetString("EFMethodName");
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
            {
                info.AddValue("EFMethodName", this.EFMethodName);
            }
        }


    }
}

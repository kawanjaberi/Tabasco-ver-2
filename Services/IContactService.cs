using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tabasco.Services
{
    public interface IContactService
    {
        List<string> GetContactsNumbers(string team);
    }
}
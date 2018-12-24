namespace Macabre2D.UI.ServiceInterfaces {

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAssemblyService {

        Task LoadAssemblies(string path);

        Task<IList<Type>> LoadTypes(Type baseType);
    }
}
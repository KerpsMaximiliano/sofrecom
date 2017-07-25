using Microsoft.Extensions.Options;
using System;

namespace Sofco.WebApi.Config
{
    public class ActiveDirectoryAuthentication
    {
        private string PathAd { get; set; }
        private string NameAttributeValue { get; set; }
        private string MailAttributeValue { get; set; }

        public ActiveDirectoryAuthentication(IOptions<ActiveDirectoryConfig> config)
        {
            PathAd = config.Value.PathAD;
            NameAttributeValue = config.Value.NameAttributeValue;
            MailAttributeValue = config.Value.EmailAttributeValue;
        }

        public string GetName(string networkUser)
        {
            return GetAttributeValue(networkUser, NameAttributeValue);
        }

        public string GetMail(string networkUser)
        {
            return GetAttributeValue(networkUser, MailAttributeValue);
        }

        public bool IsValidUser(string networkUser)
        {
            int indice = networkUser.IndexOf("\\");
            networkUser = indice > -1 ? networkUser.Substring(indice + 1) : networkUser;

            //DirectoryEntry entry = null;
            bool response = false;
            //try
            //{
            //    entry = new DirectoryEntry(PathAd);
            //    if (entry.NativeObject != null)
            //    {
            //        DirectorySearcher searcher = new DirectorySearcher(entry);
            //        searcher.Filter = String.Format(CultureInfo.InvariantCulture, "(SAMAccountName={0})", networkUser);

            //        response = searcher.FindOne() != null;

            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message, ex);
            //}
            //finally
            //{
            //    entry.Close();
            //    entry.Dispose();
            //}

            return response;
        }

        /// <summary>
        /// Obtiene un atributo especificado por parámetro de un determinado usuario de Red
        /// </summary>        
        /// <param name="usuarioRed">Usuario de Red a buscar en el Active Directory</param>
        /// <param name="usuarioRed">Atributo a buscar en el Active Directory</param>
        /// <returns></returns>
        private String GetAttributeValue(string networkUser, string attributeName)
        {
            return string.Empty;
            //int indice = networkUser.IndexOf("\\");
            //networkUser = indice > -1 ? networkUser.Substring(indice + 1) : networkUser;

            //DirectoryEntry entry = null;
            //String dato = String.Empty;
            //try
            //{
            //    entry = new DirectoryEntry(PathAd);
            //    if (entry.NativeObject != null)
            //    {
            //        DirectorySearcher searcher = new DirectorySearcher(entry);
            //        searcher.Filter = String.Format(CultureInfo.InvariantCulture, "(SAMAccountName={0})", networkUser);

            //        if (!String.IsNullOrEmpty(attributeName))
            //            searcher.PropertiesToLoad.Add(attributeName);

            //        SearchResult result = searcher.FindOne();
            //        if (null != result)
            //            dato = result.Properties[attributeName][0].ToString();
            //    }
            //}
            //catch (Exception)
            //{
            //    //throw new LdapException(ex.Message, ex);
            //}
            //finally
            //{
            //    entry.Close();
            //    entry.Dispose();
            //}

            //return dato;
        }
    }
}

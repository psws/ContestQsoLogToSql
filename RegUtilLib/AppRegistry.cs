using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Reflection;
using System.Security.Permissions;

//[assembly: RegistryPermissionAttribute(SecurityAction.RequestMinimum, RegistryPermissionAttribute.FindSecurityAttributeTypeHandle("ViewAndModify")
//    ViewAndModify = "HKEY_CURRENT_USER")]

namespace RegUtilLib
{

    public class AppRegistryHKLM
    {
        static private string Subkey;
        private RegistryKey _rkHKLM = null;

        public  AppRegistryHKLM(string sSubkey)
        {
            Subkey = sSubkey;
        }

        private RegistryKey rkHKLM
        {
            get
            {
                if (_rkHKLM == null)
                    _rkHKLM = Registry.CurrentUser;

                return _rkHKLM;
            }
        }

        public RegistryKey rkRun
        {
            get
            {
                using (_rkHKLM)
                {
                RegistryKey rkRun = rkHKLM.CreateSubKey(Subkey);


                return rkRun;
                }
            }
        }

        
    }
}

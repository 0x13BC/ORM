// <copyright file="RequeteMySqlTest.cs">Copyright ©  2017</copyright>
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ORM;

namespace ORM.Tests
{
    /// <summary>Cette classe contient des tests unitaires paramétrables pour RequeteMySql</summary>
    [PexClass(typeof(RequeteMySql))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class RequeteMySqlTest
    {
        /// <summary>Stub de test pour updateMySql(String, String[], String[], String[], String[], String)</summary>
        [PexMethod]
        internal string updateMySqlTest(
            [PexAssumeUnderTest]RequeteMySql target,
            string nomtable,
            string[] proprietes,
            string[] values,
            string[] whereprop,
            string[] whereval,
            string order
        )
        {
            string result = 
                           target.updateMySql(nomtable, proprietes, values, whereprop, whereval, order);
            return result;
            // TODO: ajouter des assertions à méthode RequeteMySqlTest.updateMySqlTest(RequeteMySql, String, String[], String[], String[], String[], String)
        }
    }
}

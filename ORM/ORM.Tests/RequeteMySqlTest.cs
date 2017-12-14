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
        /// <summary>Stub de test pour insertMySql(String, String[], String[])</summary>
        [PexMethod]
        internal string insertMySqlTest(
            [PexAssumeUnderTest]RequeteMySql target,
            string nomtable,
            string[] proprietes,
            string[] values
        )
        {
            string result = target.insertMySql(nomtable, proprietes, values);
            return result;
            // TODO: ajouter des assertions à méthode RequeteMySqlTest.insertMySqlTest(RequeteMySql, String, String[], String[])
        }
    }
}

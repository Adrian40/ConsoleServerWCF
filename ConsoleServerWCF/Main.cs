using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.Serialization;

namespace ConsoleServerWCF
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface ICastles
    {
        [OperationContract(IsInitiating = true)]
        int Login(string User, string Password);
        [OperationContract]
        int Workhours();
        [OperationContract]
        string Ownersname();
        [OperationContract]
        int AddCastle(string Castlename);
        [OperationContract]
        bool Existing_castle_in_database();
        [OperationContract]
        bool Exist_CastleofUser();
        [OperationContract]
        DataTable Datatable();
        [OperationContract]
        bool Buy_Castle(string Castlename);
        [OperationContract]
        DataTable OwnedCastlesGirdViewbe();
        [OperationContract]
        DataTable NotOwnedCastlesDataGirdViewba();
        [OperationContract]
        bool WorkinCastle(int Hour, string Castlename);
        [OperationContract]
        double StateofCastle(string Castlename);
        [OperationContract]
        DataTable Winner(string Castlename);
        [OperationContract(IsTerminating = true)]
        void ResetCastle();
    }
}
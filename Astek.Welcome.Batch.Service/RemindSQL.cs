using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astek.Welcome.Batch.Service
{
    public static class RemindSQL
    {
        #region main SQL Query
        public static readonly string SQL_STATS = "SELECT * FROM( " + Environment.NewLine +
             "SELECT " + Environment.NewLine +
             "1 as Num, " + Environment.NewLine +
             "doc.ID, " + Environment.NewLine +
             "doc.ID_Category, " + Environment.NewLine +
             "doc.ID_SubCategory, " + Environment.NewLine +
             "doc.Approbation, " + Environment.NewLine +
             "doc.Test, " + Environment.NewLine +
             "doc.Date, " + Environment.NewLine +
             "usr.ID as IdUser, " + Environment.NewLine +
             "usr.EntityName, " + Environment.NewLine +
             "usr.AgencyName, " + Environment.NewLine +
             "usr.Division, " + Environment.NewLine +
             "usr.ID_Manager, " + Environment.NewLine +
             "usr.InactivityStart, " + Environment.NewLine +
             "usr.InactivityEnd, " + Environment.NewLine +
             "ud.UpdateDate, " + Environment.NewLine +
             "ud.IsRead, " + Environment.NewLine +
             "ud.IsApproved, " + Environment.NewLine +
             "ud.IsTested, " + Environment.NewLine +
             "CASE(usr.Type) " + Environment.NewLine +
             "   WHEN 'STR' THEN doc.isStructure " + Environment.NewLine +
             "   ELSE 0" + Environment.NewLine +
             "  END as IsStructure, " + Environment.NewLine +
             "CASE (usr.Type)" + Environment.NewLine +
             "   WHEN 'MET' THEN doc.isMetier " + Environment.NewLine +
             "   ELSE 0 " + Environment.NewLine +
             " END as IsMetier " + Environment.NewLine +
             "FROM Document doc " +
             "left join UserDocument ud on (doc.ID= ud.ID_Document) " + Environment.NewLine +
             "join IntitekUser usr on(usr.ID= ud.ID_IntitekUser) " + Environment.NewLine +
             "WHERE usr.Active=1 and doc.Inactif='false' and " + Environment.NewLine +
             "    (EXISTS (SELECT* FROM EntityDocument " + Environment.NewLine +
             "        WHERE EntityDocument.ID_Document=ud.ID_Document AND " + Environment.NewLine +
             "        EntityName = (SELECT EntityName FROM IntitekUser WHERE IntitekUser.ID= usr.ID) AND " + Environment.NewLine +
             "         (AgencyName= (SELECT AgencyName from IntitekUser where IntitekUser.ID= usr.ID) OR AgencyName IS NULL)) " + Environment.NewLine +
             "     OR " + Environment.NewLine +
             "        EXISTS(SELECT* FROM ProfileDocument WHERE ProfileDocument.ID_Document= ud.ID_Document AND " + Environment.NewLine +
             "            ProfileDocument.ID_Profile IN (SELECT ID_Profile FROM ProfileUser WHERE ProfileUser.ID_IntitekUser= usr.ID))) " + Environment.NewLine +
             "UNION " + Environment.NewLine +
                 "SELECT " + Environment.NewLine +
                 "2 as Num, " + Environment.NewLine +
                 "doc.ID, " + Environment.NewLine +
                 "doc.ID_Category, " + Environment.NewLine +
                 "doc.ID_SubCategory, " + Environment.NewLine +
                 "doc.Approbation, " + Environment.NewLine +
                 "doc.Test, " + Environment.NewLine +
                 "doc.Date, " + Environment.NewLine +
                 "pu.ID_IntitekUser as IdUser, " + Environment.NewLine +
                 "usr.EntityName, " + Environment.NewLine +
                 "usr.AgencyName, " + Environment.NewLine +
                 "usr.Division, " + Environment.NewLine +
                 "usr.ID_Manager, " + Environment.NewLine +
                 "usr.InactivityStart, " + Environment.NewLine +
                 "usr.InactivityEnd, " + Environment.NewLine +
                 "NULL as UpdateDate, " + Environment.NewLine +
                 "NULL as IsRead, " + Environment.NewLine +
                 "NULL as IsApproved, " + Environment.NewLine +
                 "NULL as IsTested, " + Environment.NewLine +
                 "CASE(usr.Type) " + Environment.NewLine +
                 "  WHEN 'STR' THEN doc.isStructure " + Environment.NewLine +
                 "       ELSE 0 " + Environment.NewLine +
                 "  END as IsStructure, " + Environment.NewLine +
                 "CASE (usr.Type) " + Environment.NewLine +
                 "  WHEN 'MET' THEN doc.isMetier " + Environment.NewLine +
                 "  ELSE 0 " + Environment.NewLine +
                 "  END as IsMetier " + Environment.NewLine +
                 "  FROM ProfileDocument pd " + Environment.NewLine +
                 "  left join ProfileUser pu on pu.ID_Profile= pd.ID_Profile " + Environment.NewLine +
                 "  join IntitekUser usr on(usr.ID= pu.ID_IntitekUser) " + Environment.NewLine +
                 "  left join Document doc on doc.ID= pd.ID_Document " + Environment.NewLine +
                 "  WHERE " + Environment.NewLine +
                 "  usr.Active= 1 and doc.Inactif= 'false' and doc.IsNoActionRequired= 'false' and " + Environment.NewLine +
                 "  pd.ID_Profile in (select ID_Profile from ProfileUser where ID_IntitekUser= pu.ID_IntitekUser) " + Environment.NewLine +
                 "  and (ID_Document not in (select ID_Document from  UserDocument where ID_IntitekUser = pu.ID_IntitekUser) ) " + Environment.NewLine +
             "UNION " + Environment.NewLine +
             "SELECT  " + Environment.NewLine +
             "3 as Num, " + Environment.NewLine +
             "ID_Document as ID, " + Environment.NewLine +
             "doc.ID_Category, " + Environment.NewLine +
             "doc.ID_SubCategory, " + Environment.NewLine +
             "doc.Approbation, " + Environment.NewLine +
             "doc.Test, " + Environment.NewLine +
             "doc.Date, " + Environment.NewLine +
             "usr.ID as IdUser, " + Environment.NewLine +
             "usr.EntityName, " + Environment.NewLine +
             "usr.AgencyName, " + Environment.NewLine +
             "usr.Division, " + Environment.NewLine +
             "usr.ID_Manager, " + Environment.NewLine +
             "usr.InactivityStart, " + Environment.NewLine +
             "usr.InactivityEnd, " + Environment.NewLine +
             "NULL as UpdateDate, " + Environment.NewLine +
             "NULL as IsRead, " + Environment.NewLine +
             "NULL  as IsApproved, " + Environment.NewLine +
             "NULL as IsTested, " + Environment.NewLine +
             "CASE(usr.Type) " + Environment.NewLine +
             "  WHEN 'STR' THEN doc.isStructure " + Environment.NewLine +
             "  ELSE 0 " + Environment.NewLine +
             "  END as IsStructure, " + Environment.NewLine +
             "CASE (usr.Type) " + Environment.NewLine +
             "  WHEN 'MET' THEN doc.isMetier " + Environment.NewLine +
             "  ELSE 0 " + Environment.NewLine +
             "  END as IsMetier " + Environment.NewLine +
             "  FROM EntityDocument ed " + Environment.NewLine +
             "  left join IntitekUser usr on (ed.EntityName= usr.EntityName and (ed.AgencyName= usr.AgencyName or ed.AgencyName is null))  " + Environment.NewLine +
             "  left join Document doc on doc.ID= ed.ID_Document " + Environment.NewLine +
             "  where usr.Active= 1 and doc.Inactif= 'false' AND doc.IsNoActionRequired= 'false' " +
             " AND (ID_Document not in (select ID_Document from  UserDocument where ID_IntitekUser = usr.ID) ) " + Environment.NewLine +
             "  ) as tbl " + Environment.NewLine +
             "  where(tbl.IsStructure= 1 or tbl.IsMetier= 1) " + Environment.NewLine;
        #endregion

    }
}

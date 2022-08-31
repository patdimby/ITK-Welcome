using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Domain
{
    public partial class EdmxFunction
    {
        public const string SCHEMA = "dbo";
        //Script pour RemoveAccent
       /* USE [Intitek_ng]
        GO
        SET ANSI_NULLS ON
        GO
        SET QUOTED_IDENTIFIER ON
        GO
        CREATE FUNCTION[dbo].[RemoveAccent] (@INPUT nvarchar(255))
        RETURNS varchar(255)
        AS
        BEGIN
            DECLARE @Retour varchar(255)
            SET @Retour = REPLACE(CAST(@INPUT AS varchar(255)), '&amp;', '&')
            SET @Retour = REPLACE(@Retour, '&#233;', 'é')
            SET @Retour = REPLACE(@Retour, '&#39;', '''')
            COLLATE SQL_Latin1_General_CP1253_CI_AI 

            RETURN @Retour
        END*/
        [DbFunction("Welcome.Store", "RemoveAccent")]
        public static string RemoveAccent(string INPUT)
        {
            //c# link 
            return RemoveAccentCode(INPUT);
        }
        [DbFunction("Welcome.Store", "GetCategoryLang")]
        public static string GetCategoryLang(int ID, int ID_LANG, int ID_DEFAULT_LANG)
        {
            throw new Exception("Not implemented");
        }
        [DbFunction("Welcome.Store", "GetNameDocument")]
        public static string GetNameDocument(int ID, int ID_LANG, int ID_DEFAULT_LANG)
        {
            throw new Exception("Not implemented");
        }
        [DbFunction("Welcome.Store", "GetNomOrigineFichierDocument")]
        public static string GetNomOrigineFichierDocument(int ID, int ID_LANG, int ID_DEFAULT_LANG)
        {
            throw new Exception("Not implemented");
        }
        [DbFunction("Welcome.Store", "GetSubCategoryLang")]
        public static string GetSubCategoryLang(int ID_SUBCATEGORY, int ID_LANG, int ID_DEFAULT_LANG)
        {
            throw new Exception("Not implemented");
        }

        private static string RemoveAccentCode(string str)
        {
            if (null == str) return null;
            str = str.Replace("&amp;", "&").Replace("&#233;", "e").Replace("&#39;", "'");
            var chars = str
                .Normalize(NormalizationForm.FormD)
                .ToCharArray()
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray();

            var ret = new string(chars).Normalize(NormalizationForm.FormC);
            return ret.ToLower();
        }
        
    }
}

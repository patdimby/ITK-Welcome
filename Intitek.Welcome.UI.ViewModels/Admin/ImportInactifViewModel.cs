using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class ImportInactifViewModel
    {
        public HttpPostedFileBase FileUploadXls { get; set; }
        public bool Posted { get; set; }
        public int NbRows { get; set; }
        public int NbRowsLoaded { get; set; }
        public int NbRowsNotLoaded {
            get {
                return NbRows - NbRowsLoaded;
            }
        }
        private List<string> _Errors;
        private List<int> _LinesHasError;

        public void SetEmptyError()
        {
            this._Errors = new List<string>();
            this._LinesHasError = new List<int>();
        }
        public string Filename{
            get
            {
                if (FileUploadXls != null)
                    return  FileUploadXls.FileName;
                return string.Empty;
            }
        }
        public string ErrorDisplay {
            get {
                if (_Errors != null && _Errors.Any())
                {
                    return string.Join(Environment.NewLine, this._Errors.ToArray());
                }
                return string.Empty;
            }
        }
        public void AddError(int line, string error) {
            this._LinesHasError.Add(line);
            this._Errors.Add(error);
        }
        public bool HasError(int line)
        {
            return this._LinesHasError.IndexOf(line)>=0;
        }

    }
}

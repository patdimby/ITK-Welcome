using System.Web.Optimization;

namespace Intitek.Welcome.UI.Web
{
    public class BundleConfig
    {
        // Pour plus d'informations sur le regroupement, visitez https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = false;
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-bootstrap").Include(
                        "~/Scripts/libs/jquery/jquery-3.3.1.js",
                         "~/Scripts/libs/jquery/jquery.modal-0.9.1.min.js",
                         "~/Scripts/libs/jquery/jquery-ui.min.js",
                        "~/Scripts/libs/bootstrap/js/bootstrap.bundle.js",
                        "~/Scripts/libs/bootstrap/js/bootstrapValidator.min.js",
                        "~/Scripts/libs/loading/modal-loading.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jquery-draggableJs").Include(
                "~/Scripts/libs/jquery/jquery-ui.min.js"));
            bundles.Add(new StyleBundle("~/bundles/jquery-draggableCss").Include(
                "~/Scripts/libs/jquery/jquery-ui.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
               "~/Scripts/jquery.unobtrusive*",
               "~/Scripts/jquery.validate*"));

            // Utilisez la version de développement de Modernizr pour le développement et l'apprentissage. Puis, une fois
            // prêt pour la production, utilisez l'outil de génération à l'adresse https://modernizr.com pour sélectionner uniquement les tests dont vous avez besoin.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/custom.js").Include(
               "~/Scripts/js/custom.js",
               "~/Scripts/js/_responsive.js",
               "~/Scripts/general.js",
                "~/Scripts/js.cookie.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/dialog.js").Include(
               "~/Scripts/js/deleteDialog.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/gridmvc").Include(
               "~/Scripts/gridmvc.js",
               "~/Scripts/URI.js",
               "~/Scripts/gridmvc.lang.fr.js",
               "~/Scripts/handlebars.js",
               "~/Scripts/gridmvc.custom.categories.js",
               "~/Scripts/gridmvc.custom.subcategories.js",
               "~/Scripts/gridmvc.custom.actions.js",
               "~/Scripts/gridmvc.custom.profils.js",
               "~/Scripts/gridmvc.custom.agences.js",
               "~/Scripts/gridmvc.custom.checkbox.js",
               "~/Scripts/gridmvc.custom.typeUser.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/admin-document.js").Include(
                   "~/Scripts/libs/dropzonejs/js/dropzone.min.js",
                   "~/Scripts/js/pdf.js",
                   "~/Scripts/js/newDocument.js"
           ));

            bundles.Add(new ScriptBundle("~/Script/draggable").Include(
                "~/Scripts/libs/jquery/jquery-ui.min.js",
                "~/Scripts/js/draggble.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/login").Include(
                 "~/Scripts/js/index.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/gridmvc-custom-ajax").Include(
               "~/Scripts/gridmvc.custom.ajax.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/ckeditor").Include(
               "~/Scripts/libs/ckeditor/ckeditor.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/ckeditor-adapter").Include(
               "~/Scripts/libs/ckeditor/adapters/jquery.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap-select").Include(
               "~/Scripts/libs/bootstrap-select/js/bootstrap-select.js"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap-multiselect").Include(
               "~/Scripts/bootstrap-multiselect.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap-datepicker").Include(
               "~/Scripts/libs/datepicker/bootstrap-datepicker.min.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/justgage").Include(
               "~/Scripts/raphael-2.1.4.min.js",
               "~/Scripts/justgage.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/jquery.fileDownload").Include(
               "~/Scripts/jquery.fileDownload.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/video.js").Include(
               "~/Scripts/libs/videojs/video.min.js",
               "~/Scripts/libs/videojs/videojs-ie8.min.js",
               "~/Scripts/libs/videojs/lang/fr.js",
               "~/Scripts/libs/videojs/lang/en.js"));

            bundles.Add(new ScriptBundle("~/bundles/chartjs").Include(
             "~/Scripts/libs/chartjs/js/moment.min.js",
             "~/Scripts/libs/chartjs/js/colors.js",
             "~/Scripts/libs/chartjs/js/Chart.min.js"
            ));
            bundles.Add(new StyleBundle("~/Content/admin-css").Include(
                    "~/Scripts/abm/libs/bootstrap-select/css/bootstrap-select.css"
            ));
            bundles.Add(new StyleBundle("~/Content/custom-css").Include(                 
                  "~/Content/css/custom.css",
                "~/Content/css/_responsive.css"               
                
            ));
            bundles.Add(new StyleBundle("~/Content/sidebar-css").Include(
                    "~/Content/css/sidebar.css"
            ));

            bundles.Add(new StyleBundle("~/Content/draggable").Include(
                   "~/Scripts/libs/jquery/jquery-ui.min.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                       "~/Scripts/libs/jquery/jquery-ui.min.css",
                      "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/frontcss").Include(
                     "~/Content/front.css"));
            bundles.Add(new StyleBundle("~/Content/backcss").Include(
                     "~/Content/back.css",
                     "~/Content/glyphicon.css"));
            bundles.Add(new StyleBundle("~/Content/modalcss").Include(
                     "~/Content/modal.css"));

            bundles.Add(new StyleBundle("~/Content/gridmvc").Include(
                   "~/Content/Gridmvc.css",
                   "~/Content/gridmvc.responsive.css",
                   "~/Content/gridmvc.datepicker.css"));
            bundles.Add(new StyleBundle("~/Content/documents").Include(
                   "~/Content/document.css"));
            bundles.Add(new StyleBundle("~/Content/bootstrap-multiselect").Include(
               "~/Content/bootstrap-multiselect.css"
            ));
            bundles.Add(new StyleBundle("~/Content/video.js").Include(
              "~/Content/videojs/video-js.min.css",
              "~/Content/videojs/fantasy/index.css"));

            foreach (Bundle bundle in bundles)
            {
                bundle.WithLastModifiedToken();
            }
        }
    }
}

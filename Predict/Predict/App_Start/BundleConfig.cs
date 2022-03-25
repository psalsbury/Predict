using System.Web.Optimization;

namespace Predict
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations =
                false; // Ensures the minification doesnt occur so relative pathnames work in the css//
            bundles.Add(new StyleBundle("~/Bundles/css")
                .Include("~/Content/css/bootstrap.min.css")
                //.Include("~/Content/css/bootstrap-select.css")
                //.Include("~/Content/css/bootstrap-datepicker3.min.css")
                //.Include("~/Content/css/icheck/blue.min.css")
                .Include("~/Content/css/AdminLTE.css")
                .Include("~/Content/css/skins/skin-blue.css")
                .Include("~/Content/css/font-awesome.min.css"));
                //.Include("~/plugins/datatables/dataTables.bootstrap.min.css"));

            bundles.Add(new ScriptBundle("~/Bundles/jquery")
                .Include("~/Content/js/plugins/jquery/jquery-3.3.1.js"));

            bundles.Add(new ScriptBundle("~/Bundles/js")
                .Include("~/Content/js/plugins/bootstrap/bootstrap.js")
                //.Include("~/Content/js/plugins/fastclick/fastclick.js")
                //.Include("~/Content/js/plugins/slimscroll/jquery.slimscroll.js")
                //.Include("~/Content/js/plugins/bootstrap-select/bootstrap-select.js")
                .Include("~/Content/js/plugins/moment/moment.js")
                ////.Include("~/Content/js/plugins/datepicker/bootstrap-datepicker.js")
                //.Include("~/Content/js/plugins/icheck/icheck.js")
                //.Include("~/Content/js/plugins/validator/validator.js")
                //.Include("~/Content/js/plugins/inputmask/jquery.inputmask.bundle.js")
                .Include("~/Content/js/adminlte.js")
                //.Include("~/plugins/datatables/jquery.dataTables.js")
                //.Include("~/plugins/datatables/dataTables.bootstrap.min.js")
                .Include("~/Content/js/init.js"));
        }
    }
}
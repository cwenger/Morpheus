using System.Windows.Forms;
using Agilent.MassSpectrometry.CommonControls.AgtFolderSelectionDialog;
using Agilent.MassSpectrometry.DataAnalysis;
using Agilent.MassSpectrometry.DataAnalysis.FileDialogControls;
using Agilent.MassSpectrometry.DataAnalysis.Qualitative;

namespace Morpheus
{
    public partial class frmMain
    {
        partial void AddAgilent()
        {
            PSetReferences[] pset_references = new[] { new PSetReferences() };
            PSetFileLocations pset_file_locations = new PSetFileLocations();
            pset_file_locations.SelectedFiles = new CoreList<string>();
            pset_references[0].OriginalPSet = pset_file_locations;
            pset_references[0].CurrentPSet = pset_file_locations.Clone();

            QualFileDialogOptionsControl options = new QualFileDialogOptionsControl();
            options.Initialize(DIALOG_LABEL, CASE_INSENSITIVE_EXTENSION, CoreUtilities.GetDADefaultDataPath(), new[] { string.Empty });
            options.ParameterSets = pset_references;

            AgtDialog afsd = new AgtDialog();
            afsd.AllowMultiSelect = true;
            afsd.AppPlugIn = options;
            afsd.Initialize(DialogMode.Open);

            if(afsd.ShowDialog() == DialogResult.OK)
            {
                foreach(string data_filepath in afsd.SelectedFilePaths)
                {
                    if(!lstData.Items.Contains(data_filepath))
                    {
                        lstData.Items.Add(data_filepath);
                        tspbProgress.Value = tspbProgress.Minimum;
                    }
                }
            }
        }
    }
}

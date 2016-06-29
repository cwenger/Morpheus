using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace Morpheus
{
    class Program
    {
        internal static string GetProductNameAndVersion()
        {
            string name = Application.ProductName;
            int revision = new Version(Application.ProductVersion).Revision;
            string name_and_version = name.Insert(name.IndexOf(')'), ", revision " + revision.ToString());
            return name_and_version;
        }

        static void Main(string[] args)
        {
            if(args.Length > 0)
            {
                Arguments arguments = new Arguments(args);
                List<string> data = new List<string>(arguments["d"].Split(','));
                int min_assumed_precursor_charge_state = 2;
                if(arguments["minprecz"] != null)
                {
                    min_assumed_precursor_charge_state = int.Parse(arguments["minprecz"]);
                }
                int max_assumed_precursor_charge_state = 4;
                if(arguments["maxprecz"] != null)
                {
                    max_assumed_precursor_charge_state = int.Parse(arguments["maxprecz"]);
                }
                double abs_threshold = -1.0;
                if(arguments["at"] != null)
                {
                    abs_threshold = double.Parse(arguments["at"], CultureInfo.InvariantCulture);
                }
                double rel_threshold_percent = -1.0;
                if(arguments["rt"] != null)
                {
                    rel_threshold_percent = double.Parse(arguments["rt"], CultureInfo.InvariantCulture);
                }
                int max_peaks = 400;
                if(arguments["mp"] != null)
                {
                    max_peaks = int.Parse(arguments["mp"]);
                }
                bool assign_charge_states = true;
                if(arguments["acs"] != null)
                {
                    assign_charge_states = bool.Parse(arguments["acs"]);
                }
                bool deisotope = true;
                if(arguments["di"] != null)
                {
                    deisotope = bool.Parse(arguments["di"]);
                }
                string database = arguments["db"];
                Dictionary<string, Modification> known_variable_modifications = null;
                HashSet<Modification> variable_mods = new HashSet<Modification>();
                if(Path.GetExtension(database).Equals(".xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    bool no_uniprot_mods = false;
                    if(arguments["noup"] != null)
                    {
                        no_uniprot_mods = bool.Parse(arguments["noup"]);
                    }
                    if (!no_uniprot_mods)
                    {
                        known_variable_modifications = ProteomeDatabaseReader.ReadUniProtXmlModifications(database);
                        variable_mods.UnionWith(known_variable_modifications.Values);
                    }
                }
                bool append_decoys = false;
                if(arguments["ad"] != null)
                {
                    append_decoys = bool.Parse(arguments["ad"]);
                }
                else
                {
                    append_decoys = Path.GetExtension(database).Equals(".xml", StringComparison.InvariantCultureIgnoreCase) || !ProteomeDatabaseReader.HasDecoyProteins(database);
                }
                ProteaseDictionary proteases = ProteaseDictionary.Instance;
                Protease protease = proteases["trypsin (no proline rule)"];
                if(arguments["p"] != null)
                {
                    protease = proteases[arguments["p"]];
                }
                int max_missed_cleavages = 2;
                if(arguments["mmc"] != null)
                {
                    max_missed_cleavages = int.Parse(arguments["mmc"]);
                }
                InitiatorMethionineBehavior initiator_methionine_behavior = InitiatorMethionineBehavior.Variable;
                if(arguments["imb"] != null)
                {
                    initiator_methionine_behavior = (InitiatorMethionineBehavior)Enum.Parse(typeof(InitiatorMethionineBehavior), arguments["imb"], true);
                }
                ModificationDictionary mods = ModificationDictionary.Instance;
                List<Modification> fixed_mods = new List<Modification>();
                if(arguments["fm"] != null)
                {
                    foreach(string fixed_mod in arguments["fm"].Split(';'))
                    {
                        fixed_mods.Add(mods[fixed_mod]);
                    }
                }
                if(arguments["vm"] != null)
                {
                    foreach(string variable_mod in arguments["vm"].Split(';'))
                    {
                        Modification mod;
                        if(!mods.TryGetValue(variable_mod, out mod))
                        {
                            known_variable_modifications.TryGetValue(variable_mod, out mod);
                        }
                        variable_mods.Add(mod);
                    }
                }
                int max_variable_mod_isoforms_per_peptide = 1024;
                if(arguments["mvmi"] != null)
                {
                    max_variable_mod_isoforms_per_peptide = int.Parse(arguments["mvmi"]);
                }
                double precursor_mass_tolerance_value = 2.1;
                if(arguments["precmtv"] != null)
                {
                    precursor_mass_tolerance_value = double.Parse(arguments["precmtv"], CultureInfo.InvariantCulture);
                }
                MassToleranceUnits precursor_mass_tolerance_units = MassToleranceUnits.Da;
                if(arguments["precmtu"] != null)
                {
                    precursor_mass_tolerance_units = (MassToleranceUnits)Enum.Parse(typeof(MassToleranceUnits), arguments["precmtu"], true);
                }
                MassTolerance precursor_mass_tolerance = new MassTolerance(precursor_mass_tolerance_value, precursor_mass_tolerance_units);
                MassType precursor_mass_type = MassType.Monoisotopic;
                if(arguments["precmt"] != null)
                {
                    precursor_mass_type = (MassType)Enum.Parse(typeof(MassType), arguments["precmt"], true);
                }
                double product_mass_tolerance_value = 0.015;
                if(arguments["prodmtv"] != null)
                {
                    product_mass_tolerance_value = double.Parse(arguments["prodmtv"], CultureInfo.InvariantCulture);
                }
                MassToleranceUnits product_mass_tolerance_units = MassToleranceUnits.Da;
                if(arguments["prodmtu"] != null)
                {
                    product_mass_tolerance_units = (MassToleranceUnits)Enum.Parse(typeof(MassToleranceUnits), arguments["prodmtu"], true);
                }
                MassTolerance product_mass_tolerance = new MassTolerance(product_mass_tolerance_value, product_mass_tolerance_units);
                MassType product_mass_type = MassType.Monoisotopic;
                if(arguments["prodmt"] != null)
                {
                    product_mass_type = (MassType)Enum.Parse(typeof(MassType), arguments["prodmt"], true);
                }
                double max_fdr = 0.01;
                if(arguments["fdr"] != null)
                {
                    max_fdr = double.Parse(arguments["fdr"], CultureInfo.InvariantCulture) / 100.0;
                }
                bool consider_mods_unique = false;
                if(arguments["cmu"] != null)
                {
                    consider_mods_unique = bool.Parse(arguments["cmu"]);
                }
                int max_threads = Environment.ProcessorCount;
                if(arguments["mt"] != null)
                {
                    max_threads = int.Parse(arguments["mt"]);
                }
                bool minimize_memory_usage = false;
                if(arguments["mmu"] != null)
                {
                    minimize_memory_usage = bool.Parse(arguments["mmu"]);
                }
                string output_folder = Environment.CurrentDirectory;
                if(arguments["o"] != null)
                {
                    output_folder = arguments["o"];
                }

                List<double> massErrors = new List<double>();
                massErrors.Add(0);

                DatabaseSearcher database_searcher = new DatabaseSearcher(data,
                    min_assumed_precursor_charge_state, max_assumed_precursor_charge_state,
                    abs_threshold, rel_threshold_percent, max_peaks,
                    assign_charge_states, deisotope,
                    database, append_decoys,
                    protease, max_missed_cleavages, initiator_methionine_behavior,
                    fixed_mods, variable_mods, max_variable_mod_isoforms_per_peptide,
                    precursor_mass_tolerance, precursor_mass_type,
                    product_mass_tolerance, product_mass_type,
                    max_fdr, consider_mods_unique,
                    max_threads, minimize_memory_usage,
                    output_folder, massErrors);

                database_searcher.Starting += HandleStarting;
                database_searcher.StartingFile += HandleStartingFile;
                database_searcher.UpdateStatus += HandleUpdateStatus;
                database_searcher.UpdateProgress += HandleUpdateProgress;
                database_searcher.ThrowException += HandleThrowException;
                database_searcher.FinishedFile += HandleFinishedFile;
                database_searcher.Finished += HandleFinished;

                database_searcher.Search();
            }
            else
            {
                Console.WriteLine(Program.GetProductNameAndVersion() + " USAGE");
            }
        }

        static void HandleStarting(object sender, EventArgs e)
        {
            Console.WriteLine("Starting...");
        }

        static void HandleStartingFile(object sender, FilepathEventArgs e)
        {
            Console.WriteLine("Starting " + e.Filepath + "...");
        }

        static void HandleUpdateStatus(object sender, StatusEventArgs e)
        {
            Console.WriteLine(e.Status);
        }

        static void HandleUpdateProgress(object sender, ProgressEventArgs e)
        {
            if(e.Progress > 0)
            {
                Console.Write(e.Progress.ToString() + '%');
                if(e.Progress != 100)
                {
                    Console.Write("...");
                }
                else
                {
                    Console.WriteLine();
                }
            }
        }

        static void HandleThrowException(object sender, ExceptionEventArgs e)
        {
            Console.WriteLine(e.Exception.ToString());
        }

        static void HandleFinishedFile(object sender, FilepathEventArgs e)
        {
            Console.WriteLine("Finished " + e.Filepath + '!');
        }

        static void HandleFinished(object sender, EventArgs e)
        {
            Console.WriteLine("Finished!");
        }
    }
}
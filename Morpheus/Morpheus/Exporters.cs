using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Morpheus
{
    public static class Exporters
    {
        public static void WriteToTabDelimitedTextFile<T>(IEnumerable<T> items, string filepath)
        {
            using(StreamWriter output = new StreamWriter(filepath))
            {
                if(typeof(T) == typeof(PeptideSpectrumMatch))
                {
                    output.WriteLine(PeptideSpectrumMatch.Header);
                }
                else if(typeof(T) == typeof(PeptideSpectrumMatch))
                {
                    output.WriteLine(ProteinGroup.Header);
                }
                else if(typeof(T) == typeof(IdentificationWithFalseDiscoveryRate<PeptideSpectrumMatch>))
                {
                    output.WriteLine(PeptideSpectrumMatch.Header + IdentificationWithFalseDiscoveryRate<PeptideSpectrumMatch>.Header);
                }
                else if(typeof(T) == typeof(IdentificationWithFalseDiscoveryRate<ProteinGroup>))
                {
                    output.WriteLine(ProteinGroup.Header + IdentificationWithFalseDiscoveryRate<ProteinGroup>.Header);
                }
                foreach(T item in items)
                {
                    output.WriteLine(item.ToString());
                }
            }
        }

        public static void WritePsmsToPepXmlFile(string outputFilepath,
            string dataFilepath,
            int minimumAssumedPrecursorChargeState, int maximumAssumedPrecursorChargeState,
            double absoluteThreshold, double relativeThresholdPercent, int maximumNumberOfPeaks,
            bool assignChargeStates, bool deisotope,
            string proteomeDatabaseFilepath, bool onTheFlyDecoys, int proteins,
            Protease protease, int maximumMissedCleavages, InitiatorMethionineBehavior initiatorMethionineBehavior,
            IEnumerable<Modification> fixedModifications, string fixedModificationsString, IEnumerable<Modification> variableModifications, string variableModificationsString, int maximumVariableModificationIsoforms,
            MassTolerance precursorMassTolerance, MassType precursorMassType,
            bool precursorMonoisotopicPeakCorrection, int minimumPrecursorMonoisotopicPeakOffset, int maximumPrecursorMonoisotopicPeakOffset,
            MassTolerance productMassTolerance, MassType productMassType,
            double maximumFalseDiscoveryRate, bool considerModifiedFormsAsUniquePeptides,
            int maximumThreads, bool minimizeMemoryUsage,
            string outputFolder,
            IEnumerable<IdentificationWithFalseDiscoveryRate<PeptideSpectrumMatch>> psms)
        {
            using(XmlTextWriter output = new XmlTextWriter(outputFilepath, Encoding.UTF8))
            {
                output.Formatting = Formatting.Indented;

                output.WriteStartDocument();

                //output.WriteDocType("msms_pipeline_analysis", "-//NCBI//pepXML/EN", "pepXML.dtd", null);

                output.WriteStartElement("msms_pipeline_analysis");
                output.WriteAttributeString("date", DateTime.Now.ToString("s"));
                output.WriteAttributeString("summary_xml", outputFilepath);
                output.WriteAttributeString("xmlns", "http://regis-web.systemsbiology.net/pepXML");
                output.WriteStartElement("msms_run_summary");
                output.WriteAttributeString("base_name", Path.ChangeExtension(dataFilepath, null));
                output.WriteAttributeString("raw_data_type", Path.GetExtension(dataFilepath));
                output.WriteAttributeString("raw_data", Path.GetExtension(dataFilepath));
                output.WriteStartElement("sample_enzyme");
                output.WriteAttributeString("name", protease.Name);
                output.WriteStartElement("specificity");
                output.WriteAttributeString("sense", protease.Sense);
                output.WriteAttributeString("cut", protease.Cut);
                output.WriteAttributeString("no_cut", protease.NoCut);
                output.WriteEndElement();  // specificity
                output.WriteEndElement();  // sample_enzyme
                output.WriteStartElement("search_summary");
                output.WriteAttributeString("base_name", Path.ChangeExtension(dataFilepath, null));
                output.WriteAttributeString("search_engine", "Morpheus");
                output.WriteAttributeString("precursor_mass_type", precursorMassType.ToString().ToLower());
                output.WriteAttributeString("fragment_mass_type", productMassType.ToString().ToLower());
                output.WriteAttributeString("search_id", "1");  // not sure what this should be
                output.WriteStartElement("search_database");
                output.WriteAttributeString("local_path", proteomeDatabaseFilepath);
                output.WriteAttributeString("size_in_db_entries", proteins.ToString());
                output.WriteAttributeString("type", "AA");
                output.WriteEndElement();  // search_database
                output.WriteStartElement("enzymatic_search_constraint");
                output.WriteAttributeString("enzyme", protease.Name);
                output.WriteAttributeString("max_num_internal_cleavages", maximumMissedCleavages.ToString());
                output.WriteAttributeString("min_number_termini", protease.CleavageSpecificity.GetMinNumberTermini().ToString());
                output.WriteEndElement();  // enzymatic_search_constraint
                foreach(Modification fixed_mod in fixedModifications)
                {
                    if(fixed_mod.Type == ModificationType.AminoAcidResidue)
                    {
                        output.WriteStartElement("aminoacid_modification");
                        output.WriteAttributeString("aminoacid", fixed_mod.AminoAcid.ToString());
                        output.WriteAttributeString("massdiff", fixed_mod.MonoisotopicMassShift.ToString(CultureInfo.InvariantCulture));  // which kind of mass to use?
                        output.WriteAttributeString("mass", (AminoAcidMasses.GetMonoisotopicMass(fixed_mod.AminoAcid) + fixed_mod.MonoisotopicMassShift).ToString(CultureInfo.InvariantCulture));  // which kind of mass to use?
                        output.WriteAttributeString("variable", "N");
                        output.WriteAttributeString("description", fixed_mod.Description);
                        output.WriteEndElement();  // aminoacid_modification
                    }
                    else
                    {
                        output.WriteStartElement("terminal_modification");
                        Terminus terminus = fixed_mod.Type == ModificationType.ProteinNTerminus || fixed_mod.Type == ModificationType.PeptideNTerminus ? Terminus.N : Terminus.C;
                        output.WriteAttributeString("terminus", terminus.ToString().ToLower());
                        output.WriteAttributeString("massdiff", fixed_mod.MonoisotopicMassShift.ToString(CultureInfo.InvariantCulture));  // which kind of mass to use?
                        output.WriteAttributeString("mass", ((terminus == Terminus.N ? Constants.PEPTIDE_N_TERMINAL_MONOISOTOPIC_MASS : Constants.PEPTIDE_C_TERMINAL_MONOISOTOPIC_MASS) + fixed_mod.MonoisotopicMassShift).ToString(CultureInfo.InvariantCulture));  // which kind of mass to use?
                        output.WriteAttributeString("variable", "N");
                        output.WriteAttributeString("protein_terminus", fixed_mod.Type == ModificationType.ProteinNTerminus || fixed_mod.Type == ModificationType.ProteinCTerminus ? "Y" : "N");  // "whether modification can reside only at protein terminus (specified n or c)" (http://sashimi.sourceforge.net/schema_revision/pepXML/pepXML_v114.xsd)
                        output.WriteAttributeString("description", fixed_mod.Description);
                        output.WriteEndElement();  // terminal_modification
                    }
                }
                foreach(Modification variable_mod in variableModifications)
                {
                    if(variable_mod.Type == ModificationType.AminoAcidResidue)
                    {
                        output.WriteStartElement("aminoacid_modification");
                        output.WriteAttributeString("aminoacid", variable_mod.AminoAcid.ToString());
                        output.WriteAttributeString("massdiff", variable_mod.MonoisotopicMassShift.ToString(CultureInfo.InvariantCulture));  // which kind of mass to use?
                        output.WriteAttributeString("mass", (AminoAcidMasses.GetMonoisotopicMass(variable_mod.AminoAcid) + variable_mod.MonoisotopicMassShift).ToString(CultureInfo.InvariantCulture));  // which kind of mass to use?
                        output.WriteAttributeString("variable", "Y");
                        output.WriteAttributeString("description", variable_mod.Description);
                        output.WriteEndElement();  // aminoacid_modificationdc
                    }
                    else
                    {
                        output.WriteStartElement("terminal_modification");
                        Terminus terminus = variable_mod.Type == ModificationType.ProteinNTerminus || variable_mod.Type == ModificationType.PeptideNTerminus ? Terminus.N : Terminus.C;
                        output.WriteAttributeString("terminus", terminus.ToString().ToLower());
                        output.WriteAttributeString("massdiff", variable_mod.MonoisotopicMassShift.ToString(CultureInfo.InvariantCulture));  // which kind of mass to use?
                        output.WriteAttributeString("mass", ((terminus == Terminus.N ? Constants.PEPTIDE_N_TERMINAL_MONOISOTOPIC_MASS : Constants.PEPTIDE_C_TERMINAL_MONOISOTOPIC_MASS) + variable_mod.MonoisotopicMassShift).ToString(CultureInfo.InvariantCulture));  // which kind of mass to use?
                        output.WriteAttributeString("variable", "Y");
                        output.WriteAttributeString("protein_terminus", variable_mod.Type == ModificationType.ProteinNTerminus || variable_mod.Type == ModificationType.ProteinCTerminus ? "Y" : "N");  // "whether modification can reside only at protein terminus (specified n or c)" (http://sashimi.sourceforge.net/schema_revision/pepXML/pepXML_v114.xsd)
                        output.WriteAttributeString("description", variable_mod.Description);
                        output.WriteEndElement();  // terminal_modification
                    }
                }
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "Input Data File");
                output.WriteAttributeString("value", dataFilepath);
                output.WriteEndElement();  // parameter
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "Unknown Precursor Charge State Range");
                output.WriteAttributeString("value", minimumAssumedPrecursorChargeState.ToString("+0;-0;0") + ".." + maximumAssumedPrecursorChargeState.ToString("+0;-0;0"));
                output.WriteEndElement();  // parameter
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "Absolute MS/MS Intensity Threshold");
                output.WriteAttributeString("value", (absoluteThreshold >= 0.0 ? absoluteThreshold.ToString(CultureInfo.InvariantCulture) : "disabled"));
                output.WriteEndElement();  // parameter
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "Relative MS/MS Intensity Threshold");
                output.WriteAttributeString("value", (relativeThresholdPercent >= 0.0 ? relativeThresholdPercent.ToString(CultureInfo.InvariantCulture) + '%' : "disabled"));
                output.WriteEndElement();  // parameter
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "Maximum Number of MS/MS Peaks");
                output.WriteAttributeString("value", (maximumNumberOfPeaks >= 0 ? maximumNumberOfPeaks.ToString() : "disabled"));
                output.WriteEndElement();  // parameter
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "Assign Charge States");
                output.WriteAttributeString("value", assignChargeStates.ToString().ToLower());
                output.WriteEndElement();  // parameter
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "De-isotope");
                output.WriteAttributeString("value", deisotope.ToString().ToLower());
                output.WriteEndElement();  // parameter
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "Protein FASTA Database");
                output.WriteAttributeString("value", proteomeDatabaseFilepath);
                output.WriteEndElement();  // parameter
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "Create Target–Decoy Database On The Fly");
                output.WriteAttributeString("value", onTheFlyDecoys.ToString().ToLower());
                output.WriteEndElement();  // parameter
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "Protease");
                output.WriteAttributeString("value", protease.ToString());
                output.WriteEndElement();  // parameter
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "Maximum Missed Cleavages");
                output.WriteAttributeString("value", maximumMissedCleavages.ToString());
                output.WriteEndElement();  // parameter
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "Initiator Methionine Behavior");
                output.WriteAttributeString("value", initiatorMethionineBehavior.ToString().ToLower());
                output.WriteEndElement();  // parameter
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "Fixed Modifications");
                output.WriteAttributeString("value", fixedModificationsString);
                output.WriteEndElement();  // parameter
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "Variable Modifications");
                output.WriteAttributeString("value", variableModificationsString);
                output.WriteEndElement();  // parameter
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "Maximum Variable Modification Isoforms Per Peptide");
                output.WriteAttributeString("value", maximumVariableModificationIsoforms.ToString());
                output.WriteEndElement();  // parameter
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "Precursor Mass Tolerance");
                output.WriteAttributeString("value", '±' + precursorMassTolerance.Value.ToString(CultureInfo.InvariantCulture) + ' ' + precursorMassTolerance.Units.ToString() + " (" + precursorMassType.ToString().ToLower() + ')');
                output.WriteEndElement();  // parameter
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "Precursor Monoisotopic Peak Correction");
                output.WriteAttributeString("value", (precursorMonoisotopicPeakCorrection ? minimumPrecursorMonoisotopicPeakOffset.ToString("+0;-0;0") + ".." + maximumPrecursorMonoisotopicPeakOffset.ToString("+0;-0;0") : "disabled"));
                output.WriteEndElement();  // parameter
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "Product Mass Tolerance");
                output.WriteAttributeString("value", '±' + productMassTolerance.Value.ToString(CultureInfo.InvariantCulture) + ' ' + productMassTolerance.Units.ToString() + " (" + productMassType.ToString().ToLower() + ')');
                output.WriteEndElement();  // parameter
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "Maximum False Discovery Rate");
                output.WriteAttributeString("value", (maximumFalseDiscoveryRate * 100).ToString(CultureInfo.InvariantCulture) + '%');
                output.WriteEndElement();  // parameter
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "Consider Modified Forms as Unique Peptides");
                output.WriteAttributeString("value", considerModifiedFormsAsUniquePeptides.ToString().ToLower());
                output.WriteEndElement();  // parameter
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "Maximum Threads");
                output.WriteAttributeString("value", maximumThreads.ToString());
                output.WriteEndElement();  // parameter
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "Minimize Memory Usage");
                output.WriteAttributeString("value", minimizeMemoryUsage.ToString().ToLower());
                output.WriteEndElement();  // parameter
                output.WriteStartElement("parameter");
                output.WriteAttributeString("name", "Output Folder");
                output.WriteAttributeString("value", outputFolder.ToString());
                output.WriteEndElement();  // parameter
                output.WriteEndElement();  // search_summary

                int index = 1;
                foreach(IdentificationWithFalseDiscoveryRate<PeptideSpectrumMatch> psm_with_fdr in psms)
                {
                    PeptideSpectrumMatch psm = psm_with_fdr.Identification;
                    output.WriteStartElement("spectrum_query");
                    output.WriteAttributeString("spectrum", Path.GetFileNameWithoutExtension(psm.Spectrum.Filename) + '.' + psm.Spectrum.SpectrumNumber.ToString() + '.' + psm.Spectrum.SpectrumNumber.ToString() + '.' + psm.Spectrum.PrecursorCharge.ToString());
                    output.WriteAttributeString("start_scan", psm.Spectrum.SpectrumNumber.ToString());
                    output.WriteAttributeString("end_scan", psm.Spectrum.SpectrumNumber.ToString());
                    if(!double.IsNaN(psm.Spectrum.RetentionTimeMinutes))
                    {
                        output.WriteAttributeString("retention_time_sec", TimeSpan.FromMinutes(psm.Spectrum.RetentionTimeMinutes).TotalSeconds.ToString(CultureInfo.InvariantCulture));
                    }
                    //output.WriteAttributeString("activation_method", psm.Spectrum.FragmentationMethod);
                    output.WriteAttributeString("precursor_neutral_mass", psm.Spectrum.PrecursorMass.ToString(CultureInfo.InvariantCulture));
                    output.WriteAttributeString("assumed_charge", psm.Spectrum.PrecursorCharge.ToString());
                    output.WriteAttributeString("index", index.ToString());
                    output.WriteStartElement("search_result");
                    output.WriteStartElement("search_hit");
                    output.WriteAttributeString("hit_rank", "1");
                    output.WriteAttributeString("peptide", psm.Peptide.BaseSequence);
                    output.WriteAttributeString("peptide_prev_aa", psm.Peptide.PreviousAminoAcid.ToString());
                    output.WriteAttributeString("peptide_next_aa", psm.Peptide.NextAminoAcid.ToString());
                    output.WriteAttributeString("protein", psm.Peptide.Parent.Description);
                    output.WriteAttributeString("num_tot_proteins", "1");  // needs to be updated
                    output.WriteAttributeString("num_matched_ions", psm.MatchingProducts.ToString());
                    output.WriteAttributeString("tot_num_ions", psm.TotalProducts.ToString());
                    output.WriteAttributeString("calc_neutral_pep_mass", precursorMassType == MassType.Average ? psm.Peptide.AverageMass.ToString(CultureInfo.InvariantCulture) : psm.Peptide.MonoisotopicMass.ToString(CultureInfo.InvariantCulture));
                    output.WriteAttributeString("massdiff", (psm.Spectrum.PrecursorMass - (precursorMassType == MassType.Average ? psm.Peptide.AverageMass : psm.Peptide.MonoisotopicMass)).ToString(CultureInfo.InvariantCulture));
                    output.WriteAttributeString("is_rejected", "0");
                    output.WriteAttributeString("protein_descr", psm.Peptide.Parent.Description);
                    if((psm.Peptide.FixedModifications != null && psm.Peptide.FixedModifications.Count > 0) || (psm.Peptide.VariableModifications != null && psm.Peptide.VariableModifications.Count > 0))
                    {
                        output.WriteStartElement("modification_info");
                        output.WriteAttributeString("modified_peptide", psm.Peptide.BaseSequence);
                        for(int i = 0; i < psm.Peptide.Length; i++)
                        {
                            double mass_shift = 0.0;
                            if(psm.Peptide.FixedModifications != null && psm.Peptide.FixedModifications.ContainsKey(i + 2))
                            {
                                foreach(Modification fixed_mod in psm.Peptide.FixedModifications[i + 2])
                                {
                                    mass_shift += precursorMassType == MassType.Average ? fixed_mod.AverageMassShift : fixed_mod.MonoisotopicMassShift;
                                }
                            }
                            if(psm.Peptide.VariableModifications != null && psm.Peptide.VariableModifications.ContainsKey(i + 2))
                            {
                                mass_shift += precursorMassType == MassType.Average ? psm.Peptide.VariableModifications[i + 2].AverageMassShift : psm.Peptide.VariableModifications[i + 2].MonoisotopicMassShift;
                            }
                            if(mass_shift != 0.0)
                            {
                                output.WriteStartElement("mod_aminoacid_mass");
                                output.WriteAttributeString("position", (i + 1).ToString());
                                output.WriteAttributeString("mass", ((precursorMassType == MassType.Average ? AminoAcidMasses.GetAverageMass(psm.Peptide[i]) : AminoAcidMasses.GetMonoisotopicMass(psm.Peptide[i])) + mass_shift).ToString(CultureInfo.InvariantCulture));
                                output.WriteEndElement();  // mod_aminoacid_mass
                            }
                        }
                        output.WriteEndElement();  // modification_info
                    }
                    output.WriteStartElement("search_score");
                    output.WriteAttributeString("name", "Morpheus Score");
                    output.WriteAttributeString("value", psm.MorpheusScore.ToString(CultureInfo.InvariantCulture));
                    output.WriteEndElement();  // search_score
                    output.WriteStartElement("search_score");
                    output.WriteAttributeString("name", "PSM q-value");
                    output.WriteAttributeString("value", psm_with_fdr.QValue.ToString(CultureInfo.InvariantCulture));
                    output.WriteEndElement();  // search_score
                    output.WriteEndElement();  // search_hit
                    output.WriteEndElement();  // search_result
                    output.WriteEndElement();  // spectrum_query

                    index++;
                }
                output.WriteEndElement();  // msms_run_summary
                output.WriteEndElement();  // msms_pipeline_analysis

                output.WriteEndDocument();
            }
        }

        public static void WritePsmsToMZIdentMLFile(string outputFilepath,
            IEnumerable<string> dataFilepaths,
            int minimumAssumedPrecursorChargeState, int maximumAssumedPrecursorChargeState,
            double absoluteThreshold, double relativeThresholdPercent, int maximumNumberOfPeaks,
            bool assignChargeStates, bool deisotope,
            string proteomeDatabaseFilepath, FileStream proteomeDatabase, bool onTheFlyDecoys, int proteins,
            Protease protease, int maximumMissedCleavages, InitiatorMethionineBehavior initiatorMethionineBehavior,
            IEnumerable<Modification> fixedModifications, IEnumerable<Modification> variableModifications, int maximumVariableModificationIsoforms,
            MassTolerance precursorMassTolerance, MassType precursorMassType,
            bool precursorMonoisotopicPeakCorrection, int minimumPrecursorMonoisotopicPeakOffset, int maximumPrecursorMonoisotopicPeakOffset,
            MassTolerance productMassTolerance, MassType productMassType,
            double maximumFalseDiscoveryRate, bool considerModifiedFormsAsUniquePeptides,
            int maximumThreads, bool minimizeMemoryUsage,
            string outputFolder,
            IEnumerable<IdentificationWithFalseDiscoveryRate<PeptideSpectrumMatch>> psms,
            IEnumerable<IdentificationWithFalseDiscoveryRate<ProteinGroup>> proteinGroups)
        {
            using(XmlTextWriter output = new XmlTextWriter(outputFilepath, Encoding.UTF8))
            {
                output.Formatting = Formatting.Indented;

                output.WriteStartDocument();

                output.WriteStartElement("MzIdentML");
                output.WriteAttributeString("xmlns", null, null, "http://psidev.info/psi/pi/mzIdentML/1.1.1");
                output.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                output.WriteAttributeString("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance", "http://psidev.info/psi/pi/mzIdentML/1.1.1 https://raw.githubusercontent.com/HUPO-PSI/mzIdentML/master/schema/mzIdentML1.1.1.xsd");
                output.WriteAttributeString("creationDate", DateTime.Now.ToString("s"));
                output.WriteAttributeString("id", Path.GetFileNameWithoutExtension(outputFilepath));
                output.WriteAttributeString("version", "1.1.1");

                output.WriteStartElement("cvList");
                output.WriteStartElement("cv");
                output.WriteAttributeString("id", "PSI-MS");
                output.WriteAttributeString("fullName", "Proteomics Standards Initiative Mass Spectrometry Vocabularies");
                output.WriteAttributeString("uri", "http://psidev.cvs.sourceforge.net/viewvc/*checkout*/psidev/psi/psi-ms/mzML/controlledVocabulary/psi-ms.obo");
                output.WriteAttributeString("version", "2.25.0");
                output.WriteEndElement();  // cv
                output.WriteStartElement("cv");
                output.WriteAttributeString("id", "UO");
                output.WriteAttributeString("fullName", "UNIT-ONTOLOGY");
                output.WriteAttributeString("uri", "http://obo.cvs.sourceforge.net/*checkout*/obo/obo/ontology/phenotype/unit.obo");
                output.WriteEndElement();  // cv
                output.WriteStartElement("cv");
                output.WriteAttributeString("id", "UNIMOD");
                output.WriteAttributeString("fullName", "UNIMOD");
                output.WriteAttributeString("uri", "http://www.unimod.org/obo/unimod.obo");
                output.WriteEndElement();  // cv
                output.WriteStartElement("cv");
                output.WriteAttributeString("id", "PSI-MOD");
                output.WriteAttributeString("fullName", "PSI-MOD");
                output.WriteAttributeString("uri", "http://psidev.cvs.sourceforge.net/viewvc/psidev/psi/mod/data/PSI-MOD.obo");
                output.WriteEndElement();  // cv
                output.WriteEndElement();  // cvList

                output.WriteStartElement("AnalysisSoftwareList");
                output.WriteStartElement("AnalysisSoftware");
                output.WriteAttributeString("id", "AS_morpheus");
                output.WriteAttributeString("name", "Morpheus");
                output.WriteAttributeString("version", new Version(Application.ProductVersion).Revision.ToString());
                output.WriteAttributeString("uri", "http://cwenger.github.io/Morpheus/");
                output.WriteStartElement("SoftwareName");
                output.WriteStartElement("userParam");
                output.WriteAttributeString("name", "Morpheus");
                output.WriteEndElement();  // userParam
                output.WriteEndElement();  // SoftwareName
                output.WriteEndElement();  // AnalysisSoftware
                output.WriteEndElement();  // AnalysisSoftwareList

                output.WriteStartElement("SequenceCollection");
                foreach(Protein protein in ProteomeDatabaseReader.ReadProteins(proteomeDatabase, onTheFlyDecoys, null))
                {
                    output.WriteStartElement("DBSequence");
                    string accession = protein.Accession;
                    output.WriteAttributeString("id", "DBS_" + accession);
                    output.WriteAttributeString("accession", accession);
                    output.WriteAttributeString("searchDatabase_ref", "SDB_" + Path.GetFileNameWithoutExtension(proteomeDatabaseFilepath));
                    output.WriteAttributeString("length", protein.Length.ToString());
                    output.WriteEndElement();  // DBSequence
                }
                int index = 1;
                Dictionary<PeptideSpectrumMatch, int> ids = new Dictionary<PeptideSpectrumMatch, int>();
                foreach(IdentificationWithFalseDiscoveryRate<PeptideSpectrumMatch> psm_with_fdr in psms)
                {
                    PeptideSpectrumMatch psm = psm_with_fdr.Identification;
                    Peptide peptide = psm.Peptide;
                    output.WriteStartElement("Peptide");
                    output.WriteAttributeString("id", "P_" + index.ToString());
                    output.WriteStartElement("PeptideSequence");
                    output.WriteString(peptide.BaseSequence);
                    output.WriteEndElement();  // PeptideSequence
                    if(peptide.FixedModifications != null)
                    {
                        foreach(KeyValuePair<int, List<Modification>> fixed_mods in peptide.FixedModifications)
                        {
                            foreach(Modification fixed_mod in fixed_mods.Value)
                            {
                                output.WriteStartElement("Modification");
                                output.WriteAttributeString("location", fixed_mods.Key.ToString());
                                if(precursorMassType == MassType.Average)
                                {
                                    output.WriteAttributeString("averageMassDelta", fixed_mod.AverageMassShift.ToString());
                                }
                                else
                                {
                                    output.WriteAttributeString("monoisotopicMassDelta", fixed_mod.MonoisotopicMassShift.ToString());
                                }
                                output.WriteStartElement("cvParam");
                                output.WriteAttributeString("accession", fixed_mod.Database + ':' + fixed_mod.DatabaseAccessionNumber.ToString());
                                output.WriteAttributeString("name", fixed_mod.DatabaseName);
                                output.WriteAttributeString("cvRef", fixed_mod.Database);
                                output.WriteEndElement();  // cvParam
                                output.WriteEndElement();  // Modification
                            }
                        }
                    }
                    if(peptide.VariableModifications != null)
                    {
                        foreach(KeyValuePair<int, Modification> variable_mod_kvp in peptide.VariableModifications)
                        {
                            Modification variable_mod = variable_mod_kvp.Value;
                            output.WriteStartElement("Modification");
                            output.WriteAttributeString("location", peptide.ModificationIndexToAminoAcidPosition(variable_mod_kvp.Key).ToString());
                            if(precursorMassType == MassType.Average)
                            {
                                output.WriteAttributeString("averageMassDelta", variable_mod.AverageMassShift.ToString());
                            }
                            else
                            {
                                output.WriteAttributeString("monoisotopicMassDelta", variable_mod.MonoisotopicMassShift.ToString());
                            }
                            output.WriteStartElement("cvParam");
                            if(variable_mod.Database == "PSI-MOD")
                            {
                                output.WriteAttributeString("accession", variable_mod.Database + ':' + variable_mod.DatabaseAccessionNumber.ToString("00000"));
                            }
                            else
                            {
                                output.WriteAttributeString("accession", variable_mod.Database + ':' + variable_mod.DatabaseAccessionNumber.ToString());
                            }
                            output.WriteAttributeString("name", variable_mod.DatabaseName);
                            output.WriteAttributeString("cvRef", variable_mod.Database);
                            output.WriteEndElement();  // cvParam
                            output.WriteEndElement();  // Modification
                        }
                    }
                    output.WriteEndElement();  // Peptide
                    ids.Add(psm, index);
                    index++;
                }
                index = 1;
                foreach(IdentificationWithFalseDiscoveryRate<PeptideSpectrumMatch> psm_with_fdr in psms)
                {
                    PeptideSpectrumMatch psm = psm_with_fdr.Identification;
                    Peptide peptide = psm.Peptide;
                    output.WriteStartElement("PeptideEvidence");
                    output.WriteAttributeString("id", "PE_" + index.ToString());
                    output.WriteAttributeString("peptide_ref", "P_" + index.ToString());
                    output.WriteAttributeString("dBSequence_ref", "DBS_" + psm.Peptide.Parent.Accession);
                    output.WriteAttributeString("isDecoy", psm.Decoy.ToString().ToLower());
                    output.WriteAttributeString("start", peptide.StartResidueNumber.ToString());
                    output.WriteAttributeString("end", peptide.EndResidueNumber.ToString());
                    output.WriteAttributeString("pre", peptide.PreviousAminoAcid.ToString());
                    output.WriteAttributeString("post", peptide.NextAminoAcid.ToString());
                    output.WriteEndElement();  // PeptideEvidence
                    index++;
                }
                output.WriteEndElement();  // SequenceCollection

                output.WriteStartElement("AnalysisCollection");
                output.WriteStartElement("SpectrumIdentification");
                output.WriteAttributeString("id", "SI");
                output.WriteAttributeString("spectrumIdentificationList_ref", "SIL");
                output.WriteAttributeString("spectrumIdentificationProtocol_ref", "SIP");
                Dictionary<string, string> spectral_data_ids = new Dictionary<string, string>();
                foreach(string data_filepath in dataFilepaths)
                {
                    string spectral_data_id = "SD_" + (spectral_data_ids.Count + 1).ToString();
                    spectral_data_ids.Add(data_filepath, spectral_data_id);
                    output.WriteStartElement("InputSpectra");
                    output.WriteAttributeString("spectraData_ref", spectral_data_id);
                    output.WriteEndElement();  // InputSpectra
                }
                output.WriteStartElement("SearchDatabaseRef");
                output.WriteAttributeString("searchDatabase_ref", "SDB_" + Path.GetFileNameWithoutExtension(proteomeDatabaseFilepath));
                output.WriteEndElement();  // SearchDatabaseRef
                output.WriteEndElement();  // SpectrumIdentification
                output.WriteEndElement();  // AnalysisCollection

                output.WriteStartElement("AnalysisProtocolCollection");
                output.WriteStartElement("SpectrumIdentificationProtocol");
                output.WriteAttributeString("id", "SIP");
                output.WriteAttributeString("analysisSoftware_ref", "AS_morpheus");
                output.WriteStartElement("SearchType");
                output.WriteStartElement("cvParam");
                output.WriteAttributeString("accession", "MS:1001083");
                output.WriteAttributeString("name", "ms-ms search");
                output.WriteAttributeString("cvRef", "PSI-MS");
                output.WriteEndElement();  // cvParam
                output.WriteEndElement();  // SearchType
                output.WriteStartElement("AdditionalSearchParams");
                output.WriteStartElement("cvParam");
                if(precursorMassType == MassType.Average)
                {
                    output.WriteAttributeString("accession", "MS:1001212");
                    output.WriteAttributeString("name", "parent mass type average");
                }
                else
                {
                    output.WriteAttributeString("accession", "MS:1001211");
                    output.WriteAttributeString("name", "parent mass type mono");
                }
                output.WriteAttributeString("cvRef", "PSI-MS");
                output.WriteEndElement();  // cvParam
                output.WriteStartElement("cvParam");
                if(productMassType == MassType.Average)
                {
                    output.WriteAttributeString("accession", "MS:1001255");
                    output.WriteAttributeString("name", "fragment mass type average");
                }
                else
                {
                    output.WriteAttributeString("accession", "MS:1001256");
                    output.WriteAttributeString("name", "fragment mass type mono");
                }
                output.WriteAttributeString("cvRef", "PSI-MS");
                output.WriteEndElement();  // cvParam
                output.WriteStartElement("userParam");
                output.WriteAttributeString("name", "Minimum Assumed Precursor Charge State");
                output.WriteAttributeString("value", minimumAssumedPrecursorChargeState.ToString());
                output.WriteEndElement();  // userParam
                output.WriteStartElement("userParam");
                output.WriteAttributeString("name", "Maximum Assumed Precursor Charge State");
                output.WriteAttributeString("value", maximumAssumedPrecursorChargeState.ToString());
                output.WriteEndElement();  // userParam
                output.WriteStartElement("userParam");
                output.WriteAttributeString("name", "Absolute MS/MS Peak Intensity Threshold");
                output.WriteAttributeString("value", absoluteThreshold >= 0.0 ? absoluteThreshold.ToString() : "disabled");
                output.WriteEndElement();  // userParam
                output.WriteStartElement("userParam");
                output.WriteAttributeString("name", "Relative MS/MS Peak Intensity Threshold");
                output.WriteAttributeString("value", relativeThresholdPercent >= 0.0 ? relativeThresholdPercent.ToString() : "disabled");
                if(relativeThresholdPercent >= 0.0)
                {
                    output.WriteAttributeString("unitAccession", "UO:0000187");
                    output.WriteAttributeString("unitName", "percent");
                    output.WriteAttributeString("unitCvRef", "UO");
                }
                output.WriteEndElement();  // userParam
                output.WriteStartElement("userParam");
                output.WriteAttributeString("name", "Maximum Number of MS/MS Peaks");
                output.WriteAttributeString("value", maximumNumberOfPeaks >= 0 ? maximumNumberOfPeaks.ToString() : "disabled");
                output.WriteEndElement();  // userParam
                output.WriteStartElement("cvParam");
                output.WriteAttributeString("accession", "MS:1000034");
                output.WriteAttributeString("name", "charge deconvolution");
                output.WriteAttributeString("value", assignChargeStates.ToString().ToLower());
                output.WriteAttributeString("cvRef", "PSI-MS");
                output.WriteEndElement();  // cvParam
                output.WriteStartElement("cvParam");
                output.WriteAttributeString("accession", "MS:1000033");
                output.WriteAttributeString("name", "deisotoping");
                output.WriteAttributeString("value", deisotope.ToString().ToLower());
                output.WriteAttributeString("cvRef", "PSI-MS");
                output.WriteEndElement();  // cvParam
                output.WriteStartElement("userParam");
                output.WriteAttributeString("name", "On-the-fly Decoy Protein Generation");
                output.WriteAttributeString("value", onTheFlyDecoys.ToString().ToLower());
                output.WriteEndElement();  // userParam
                output.WriteStartElement("userParam");
                output.WriteAttributeString("name", "Initiator Methionine Behavior");
                output.WriteAttributeString("value", initiatorMethionineBehavior.ToString().ToLower());
                output.WriteEndElement();  // userParam
                output.WriteStartElement("userParam");
                output.WriteAttributeString("name", "Maximum Number of Variable Modification Isoforms per Peptide");
                output.WriteAttributeString("value", maximumVariableModificationIsoforms.ToString());
                output.WriteEndElement();  // userParam
                output.WriteStartElement("userParam");
                output.WriteAttributeString("name", "Precursor Monoisotopic Peak Correction Enabled");
                output.WriteAttributeString("value", precursorMonoisotopicPeakCorrection.ToString().ToLower());
                output.WriteEndElement();  // userParam
                output.WriteStartElement("userParam");
                output.WriteAttributeString("name", "Minimum Precursor Monoisotopic Peak Correction Offset");
                output.WriteAttributeString("value", precursorMonoisotopicPeakCorrection ? minimumPrecursorMonoisotopicPeakOffset.ToString() : "n/a");
                output.WriteEndElement();  // userParam
                output.WriteStartElement("userParam");
                output.WriteAttributeString("name", "Maximum Precursor Monoisotopic Peak Correction Offset");
                output.WriteAttributeString("value", precursorMonoisotopicPeakCorrection ? maximumPrecursorMonoisotopicPeakOffset.ToString() : "n/a");
                output.WriteEndElement();  // userParam
                output.WriteStartElement("userParam");
                output.WriteAttributeString("name", "Consider Modified Forms as Unique Peptides");
                output.WriteAttributeString("value", considerModifiedFormsAsUniquePeptides.ToString().ToLower());
                output.WriteEndElement();  // userParam
                output.WriteStartElement("userParam");
                output.WriteAttributeString("name", "Maximum Threads");
                output.WriteAttributeString("value", maximumThreads.ToString());
                output.WriteEndElement();  // userParam
                output.WriteStartElement("userParam");
                output.WriteAttributeString("name", "Minimize Memory Usage");
                output.WriteAttributeString("value", minimizeMemoryUsage.ToString().ToLower());
                output.WriteEndElement();  // userParam
                output.WriteEndElement();  // AdditionalSearchParams
                output.WriteStartElement("ModificationParams");
                foreach(Modification fixed_mod in fixedModifications)
                {
                    output.WriteStartElement("SearchModification");
                    output.WriteAttributeString("fixedMod", true.ToString().ToLower());
                    output.WriteAttributeString("massDelta", (precursorMassType == MassType.Average ? fixed_mod.AverageMassShift : fixed_mod.MonoisotopicMassShift).ToString());
                    output.WriteAttributeString("residues", fixed_mod.AminoAcid.ToString());
                    output.WriteStartElement("cvParam");
                    output.WriteAttributeString("accession", fixed_mod.Database + ':' + fixed_mod.DatabaseAccessionNumber.ToString());
                    output.WriteAttributeString("name", fixed_mod.DatabaseName);
                    output.WriteAttributeString("cvRef", fixed_mod.Database);
                    output.WriteEndElement();  // cvParam
                    output.WriteEndElement();  // SearchModification
                }
                foreach(Modification variable_mod in variableModifications)
                {
                    output.WriteStartElement("SearchModification");
                    output.WriteAttributeString("fixedMod", false.ToString().ToLower());
                    output.WriteAttributeString("massDelta", (precursorMassType == MassType.Average ? variable_mod.AverageMassShift : variable_mod.MonoisotopicMassShift).ToString());
                    output.WriteAttributeString("residues", variable_mod.AminoAcid.ToString());
                    output.WriteStartElement("cvParam");
                    if(variable_mod.Database == "PSI-MOD")
                    {
                        output.WriteAttributeString("accession", variable_mod.Database + ':' + variable_mod.DatabaseAccessionNumber.ToString("00000"));
                    }
                    else
                    {
                        output.WriteAttributeString("accession", variable_mod.Database + ':' + variable_mod.DatabaseAccessionNumber.ToString());
                    }
                    output.WriteAttributeString("name", variable_mod.DatabaseName);
                    output.WriteAttributeString("cvRef", variable_mod.Database);
                    output.WriteEndElement();  // cvParam
                    output.WriteEndElement();  // SearchModification
                }
                output.WriteEndElement();  // ModificationParams
                output.WriteStartElement("Enzymes");
                output.WriteStartElement("Enzyme");
                output.WriteAttributeString("id", "E");
                output.WriteAttributeString("name", protease.Name);
                output.WriteAttributeString("semiSpecific", (protease.CleavageSpecificity == CleavageSpecificity.Semi || protease.CleavageSpecificity == CleavageSpecificity.SemiN || protease.CleavageSpecificity == CleavageSpecificity.SemiC).ToString().ToLower());
                output.WriteAttributeString("missedCleavages", maximumMissedCleavages.ToString());
                output.WriteEndElement();  // Enzyme
                output.WriteEndElement();  // Enzymes
                output.WriteStartElement("FragmentTolerance");
                output.WriteStartElement("cvParam");
                output.WriteAttributeString("accession", "MS:1001412");
                output.WriteAttributeString("name", "search tolerance plus value");
                output.WriteAttributeString("value", productMassTolerance.Value.ToString());
                output.WriteAttributeString("cvRef", "PSI-MS");
                if(productMassTolerance.Units == MassToleranceUnits.ppm)
                {
                    output.WriteAttributeString("unitAccession", "UO:0000169");
                    output.WriteAttributeString("unitName", "parts per million");
                }
                else
                {
                    output.WriteAttributeString("unitAccession", "UO:0000221");
                    output.WriteAttributeString("unitName", "dalton");
                }
                output.WriteAttributeString("unitCvRef", "UO");
                output.WriteEndElement();  // cvParam
                output.WriteStartElement("cvParam");
                output.WriteAttributeString("accession", "MS:1001413");
                output.WriteAttributeString("name", "search tolerance minus value");
                output.WriteAttributeString("value", productMassTolerance.Value.ToString());
                output.WriteAttributeString("cvRef", "PSI-MS");
                if(productMassTolerance.Units == MassToleranceUnits.ppm)
                {
                    output.WriteAttributeString("unitAccession", "UO:0000169");
                    output.WriteAttributeString("unitName", "parts per million");
                }
                else
                {
                    output.WriteAttributeString("unitAccession", "UO:0000221");
                    output.WriteAttributeString("unitName", "dalton");
                }
                output.WriteAttributeString("unitCvRef", "UO");
                output.WriteEndElement();  // cvParam
                output.WriteEndElement();  // FragmentTolerance
                output.WriteStartElement("ParentTolerance");
                output.WriteStartElement("cvParam");
                output.WriteAttributeString("accession", "MS:1001412");
                output.WriteAttributeString("name", "search tolerance plus value");
                output.WriteAttributeString("value", precursorMassTolerance.Value.ToString());
                output.WriteAttributeString("cvRef", "PSI-MS");
                if(precursorMassTolerance.Units == MassToleranceUnits.ppm)
                {
                    output.WriteAttributeString("unitAccession", "UO:0000169");
                    output.WriteAttributeString("unitName", "parts per million");
                }
                else
                {
                    output.WriteAttributeString("unitAccession", "UO:0000221");
                    output.WriteAttributeString("unitName", "dalton");
                }
                output.WriteAttributeString("unitCvRef", "UO");
                output.WriteEndElement();  // cvParam
                output.WriteStartElement("cvParam");
                output.WriteAttributeString("accession", "MS:1001413");
                output.WriteAttributeString("name", "search tolerance minus value");
                output.WriteAttributeString("value", precursorMassTolerance.Value.ToString());
                output.WriteAttributeString("cvRef", "PSI-MS");
                if(precursorMassTolerance.Units == MassToleranceUnits.ppm)
                {
                    output.WriteAttributeString("unitAccession", "UO:0000169");
                    output.WriteAttributeString("unitName", "parts per million");
                }
                else
                {
                    output.WriteAttributeString("unitAccession", "UO:0000221");
                    output.WriteAttributeString("unitName", "dalton");
                }
                output.WriteAttributeString("unitCvRef", "UO");
                output.WriteEndElement();  // cvParam
                output.WriteEndElement();  // ParentTolerance
                output.WriteStartElement("Threshold");
                output.WriteStartElement("cvParam");
                output.WriteAttributeString("accession", "MS:1001448");
                output.WriteAttributeString("name", "pep:FDR threshold");
                output.WriteAttributeString("cvRef", "PSI-MS");
                output.WriteAttributeString("value", maximumFalseDiscoveryRate.ToString());
                output.WriteEndElement();  // cvParam
                output.WriteEndElement();  // Threshold
                output.WriteEndElement();  // SpectrumIdentificationProtocol
                output.WriteStartElement("ProteinDetectionProtocol");
                output.WriteAttributeString("id", "PDP");
                output.WriteAttributeString("analysisSoftware_ref", "AS_morpheus");
                output.WriteStartElement("Threshold");
                output.WriteStartElement("cvParam");
                output.WriteAttributeString("accession", "MS:1001447");
                output.WriteAttributeString("name", "prot:FDR threshold");
                output.WriteAttributeString("cvRef", "PSI-MS");
                output.WriteAttributeString("value", maximumFalseDiscoveryRate.ToString());
                output.WriteEndElement();  // cvParam
                output.WriteEndElement();  // Threshold
                output.WriteEndElement();  // ProteinDetectionProtocol
                output.WriteEndElement();  // AnalysisProtocolCollection

                output.WriteStartElement("DataCollection");
                output.WriteStartElement("Inputs");
                output.WriteStartElement("SearchDatabase");
                output.WriteAttributeString("id", "SDB_" + Path.GetFileNameWithoutExtension(proteomeDatabaseFilepath));
                output.WriteAttributeString("location", new Uri(proteomeDatabaseFilepath).AbsoluteUri);
                output.WriteAttributeString("numDatabaseSequences", proteins.ToString());
                if(Path.GetExtension(proteomeDatabase.Name).Equals(".xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    // todo: add UniProt XML to CV
                }
                else
                {
                    output.WriteStartElement("FileFormat");
                    output.WriteStartElement("cvParam");
                    output.WriteAttributeString("accession", "MS:1001348");
                    output.WriteAttributeString("name", "FASTA format");
                    output.WriteAttributeString("cvRef", "PSI-MS");
                    output.WriteEndElement();  // cvParam
                    output.WriteEndElement();  // FileFormat
                }
                output.WriteStartElement("DatabaseName");
                output.WriteStartElement("userParam");
                output.WriteAttributeString("name", proteomeDatabaseFilepath);
                output.WriteEndElement();  // userParam
                output.WriteEndElement();  // DatabaseName              
                output.WriteStartElement("cvParam");
                output.WriteAttributeString("accession", "MS:1001073");
                output.WriteAttributeString("name", "database type amino acid");
                output.WriteAttributeString("cvRef", "PSI-MS");
                output.WriteEndElement();  // cvParam
                output.WriteEndElement();  // SearchDatabase
                foreach(string data_filepath in dataFilepaths)
                {
                    output.WriteStartElement("SpectraData");
                    output.WriteAttributeString("id", spectral_data_ids[data_filepath]);
                    output.WriteAttributeString("location", new Uri(data_filepath).AbsoluteUri);
                    output.WriteStartElement("SpectrumIDFormat");
                    output.WriteStartElement("cvParam");
                    if(Path.GetExtension(data_filepath).ToLower() == ".raw")
                    {
                        output.WriteAttributeString("accession", "MS:1000768");
                        output.WriteAttributeString("name", "Thermo nativeID format");
                    }
                    else if(Path.GetExtension(data_filepath).ToLower() == ".d")
                    {
                        output.WriteAttributeString("accession", "MS:1001508");
                        output.WriteAttributeString("name", "Agilent MassHunter nativeID format");
                    }
                    else
                    {
                        output.WriteAttributeString("accession", "MS:1001530");
                        output.WriteAttributeString("name", "mzML unique identifier");
                    }
                    output.WriteAttributeString("cvRef", "PSI-MS");
                    output.WriteEndElement();  // cvParam
                    output.WriteEndElement();  // SpectrumIDFormat
                    output.WriteEndElement();  // SpectraData
                }
                output.WriteEndElement();  // Inputs
                output.WriteStartElement("AnalysisData");
                output.WriteStartElement("SpectrumIdentificationList");
                output.WriteAttributeString("id", "SIL");
                index = 1;
                foreach(IdentificationWithFalseDiscoveryRate<PeptideSpectrumMatch> psm_with_fdr in psms)
                {
                    PeptideSpectrumMatch psm = psm_with_fdr.Identification;
                    TandemMassSpectrum spectrum = psm.Spectrum;
                    output.WriteStartElement("SpectrumIdentificationResult");
                    output.WriteAttributeString("id", "SIR_" + index.ToString());
                    output.WriteAttributeString("spectraData_ref", spectral_data_ids[spectrum.Filename]);
                    output.WriteAttributeString("spectrumID", spectrum.SpectrumId);
                    output.WriteStartElement("SpectrumIdentificationItem");
                    output.WriteAttributeString("id", "SII_" + index.ToString());
                    output.WriteAttributeString("chargeState", spectrum.PrecursorCharge.ToString());
                    output.WriteAttributeString("experimentalMassToCharge", spectrum.PrecursorMZ.ToString());
                    output.WriteAttributeString("calculatedMassToCharge", MSPeak.MZFromMass(precursorMassType == MassType.Average ? psm.Peptide.AverageMass : psm.Peptide.MonoisotopicMass, spectrum.PrecursorCharge).ToString());
                    output.WriteAttributeString("passThreshold", (psm_with_fdr.QValue <= maximumFalseDiscoveryRate).ToString().ToLower());
                    output.WriteAttributeString("rank", "1");
                    output.WriteStartElement("PeptideEvidenceRef");
                    output.WriteAttributeString("peptideEvidence_ref", "PE_" + index.ToString());
                    output.WriteEndElement();  // PeptideEvidenceRef
                    output.WriteStartElement("userParam");
                    output.WriteAttributeString("name", "Morpheus score");
                    output.WriteAttributeString("value", psm.MorpheusScore.ToString());
                    output.WriteEndElement();  // userParam
                    output.WriteStartElement("cvParam");
                    output.WriteAttributeString("accession", "MS:1002354");
                    output.WriteAttributeString("name", "PSM-level q-value");
                    output.WriteAttributeString("cvRef", "PSI-MS");
                    output.WriteAttributeString("value", psm_with_fdr.QValue.ToString());
                    output.WriteEndElement();  // cvParam
                    output.WriteEndElement();  // SpectrumIdentificationItem
                    output.WriteEndElement();  // SpectrumIdentificationResult
                    index++;
                }
                output.WriteEndElement();  // SpectrumIdentificationList
                output.WriteStartElement("ProteinDetectionList");
                output.WriteAttributeString("id", "PDL");
                index = 1;
                foreach(IdentificationWithFalseDiscoveryRate<ProteinGroup> protein_group_with_fdr in proteinGroups)
                {
                    ProteinGroup protein_group = protein_group_with_fdr.Identification;
                    output.WriteStartElement("ProteinAmbiguityGroup");
                    output.WriteAttributeString("id", "PAG_" + index.ToString());
                    foreach(Protein protein in protein_group)
                    {
                        output.WriteStartElement("ProteinDetectionHypothesis");
                        output.WriteAttributeString("id", "PDH_" + protein.Accession);
                        output.WriteAttributeString("dBSequence_ref", "DBS_" + protein.Accession);
                        output.WriteAttributeString("passThreshold", (protein_group_with_fdr.QValue <= maximumFalseDiscoveryRate).ToString().ToLower());
                        foreach(PeptideSpectrumMatch psm in protein_group.PeptideSpectrumMatches)
                        {
                            int id = ids[psm];
                            output.WriteStartElement("PeptideHypothesis");
                            output.WriteAttributeString("peptideEvidence_ref", "PE_" + id.ToString());
                            output.WriteStartElement("SpectrumIdentificationItemRef");
                            output.WriteAttributeString("spectrumIdentificationItem_ref", "SII_" + id.ToString());
                            output.WriteEndElement();  // SpectrumIdentificationItemRef
                            output.WriteEndElement();  // PeptideHypothesis
                        }
                        output.WriteStartElement("userParam");
                        output.WriteAttributeString("name", "summed Morpheus score");
                        output.WriteAttributeString("value", protein_group.SummedMorpheusScore.ToString());
                        output.WriteEndElement();  // userParam
                        output.WriteStartElement("cvParam");
                        output.WriteAttributeString("accession", "MS:1002373");
                        output.WriteAttributeString("name", "protein group-level q-value");
                        output.WriteAttributeString("cvRef", "PSI-MS");
                        output.WriteAttributeString("value", protein_group_with_fdr.QValue.ToString());
                        output.WriteEndElement();  // cvParam
                        output.WriteStartElement("cvParam");
                        output.WriteAttributeString("accession", "MS:1001093");
                        output.WriteAttributeString("name", "sequence coverage");
                        output.WriteAttributeString("cvRef", "PSI-MS");
                        output.WriteAttributeString("value", (protein.CalculateSequenceCoverage() * 100).ToString());
                        output.WriteEndElement();  // cvParam
                        output.WriteStartElement("cvParam");
                        output.WriteAttributeString("accession", "MS:1001097");
                        output.WriteAttributeString("name", "distinct peptide sequences");
                        output.WriteAttributeString("cvRef", "PSI-MS");
                        output.WriteAttributeString("value", protein_group.UniquePeptides.Count.ToString());
                        output.WriteEndElement();  // cvParam
                        output.WriteEndElement();  // ProteinDetectionHypothesis
                    }
                    output.WriteEndElement();  // ProteinAmbiguityGroup
                    index++;
                }
                output.WriteEndElement();  // ProteinDetectionList
                output.WriteEndElement();  // AnalysisData
                output.WriteEndElement();  // DataCollection

                output.WriteEndElement();  // MzIdentML

                output.WriteEndDocument();
            }
        }
    }
}
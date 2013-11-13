//#define NON_MULTITHREADED

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Morpheus
{
    public class DatabaseSearcher
    {
        private ICollection<string> dataFilepaths;
        private int minimumAssumedPrecursorChargeState;
        private int maximumAssumedPrecursorChargeState;
        private double absoluteThreshold;
        private double relativeThresholdPercent;
        private int maximumNumberOfPeaks;
        private bool assignChargeStates;
        private bool deisotope;
        private string proteinFastaDatabaseFilepath;
        private bool onTheFlyDecoys;
        private Protease protease;
        private int maximumMissedCleavages;
        private InitiatorMethionineBehavior initiatorMethionineBehavior;
        private IEnumerable<Modification> fixedModifications;
        private IEnumerable<Modification> variableModifications;
        private int maximumVariableModificationIsoforms;
        private MassTolerance precursorMassTolerance;
        private MassType precursorMassType;
        private bool precursorMonoisotopicPeakCorrection;
        private int minimumPrecursorMonoisotopicPeakOffset;
        private int maximumPrecursorMonoisotopicPeakOffset;
        private MassTolerance productMassTolerance;
        private MassType productMassType;
        private double maximumFalseDiscoveryRate;
        private bool considerModifiedFormsAsUniquePeptides;
        private int maximumThreads;
        private bool minimizeMemoryUsage;
        private string outputFolder;

        public DatabaseSearcher(ICollection<string> dataFilepaths,
            int minimumAssumedPrecursorChargeState, int maximumAssumedPrecursorChargeState,
            double absoluteThreshold, double relativeThresholdPercent, int maximumNumberOfPeaks,
            bool assignChargeStates, bool deisotope,
            string proteinFastaDatabaseFilepath, bool onTheFlyDecoys,
            Protease protease, int maximumMissedCleavages, InitiatorMethionineBehavior initiatorMethionineBehavior,
            IEnumerable<Modification> fixedModifications, IEnumerable<Modification> variableModifications, int maximumVariableModificationIsoforms,
            MassTolerance precursorMassTolerance, MassType precursorMassType,
            bool precursorMonoisotopicPeakCorrection, int minimumPrecursorMonoisotopicPeakOffset, int maximumPrecursorMonoisotopicPeakOffset,
            MassTolerance productMassTolerance, MassType productMassType,
            double maximumFalseDiscoveryRate, bool considerModifiedFormsAsUniquePeptides,
            int maximumThreads, bool minimizeMemoryUsage,
            string outputFolder)
        {
            this.dataFilepaths = dataFilepaths;
            this.assignChargeStates = assignChargeStates;
            this.deisotope = deisotope;
            this.proteinFastaDatabaseFilepath = proteinFastaDatabaseFilepath;
            this.onTheFlyDecoys = onTheFlyDecoys;
            this.protease = protease;
            this.maximumMissedCleavages = maximumMissedCleavages;
            this.initiatorMethionineBehavior = initiatorMethionineBehavior;
            this.fixedModifications = fixedModifications;
            this.variableModifications = variableModifications;
            this.maximumVariableModificationIsoforms = maximumVariableModificationIsoforms;
            this.minimumAssumedPrecursorChargeState = minimumAssumedPrecursorChargeState;
            this.maximumAssumedPrecursorChargeState = maximumAssumedPrecursorChargeState;
            this.absoluteThreshold = absoluteThreshold;
            this.relativeThresholdPercent = relativeThresholdPercent;
            this.maximumNumberOfPeaks = maximumNumberOfPeaks;
            this.precursorMassTolerance = precursorMassTolerance;
            this.precursorMassType = precursorMassType;
            this.precursorMonoisotopicPeakCorrection = precursorMonoisotopicPeakCorrection;
            this.minimumPrecursorMonoisotopicPeakOffset = minimumPrecursorMonoisotopicPeakOffset;
            this.maximumPrecursorMonoisotopicPeakOffset = maximumPrecursorMonoisotopicPeakOffset;
            this.productMassTolerance = productMassTolerance;
            this.productMassType = productMassType;
            this.maximumFalseDiscoveryRate = maximumFalseDiscoveryRate;
            this.considerModifiedFormsAsUniquePeptides = considerModifiedFormsAsUniquePeptides;
            this.maximumThreads = maximumThreads;
            this.minimizeMemoryUsage = minimizeMemoryUsage;
            this.outputFolder = outputFolder;
        }

        public event EventHandler Starting;

        protected virtual void OnStarting(EventArgs e)
        {
            EventHandler handler = Starting;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<FilepathEventArgs> StartingFile;

        protected virtual void OnStartingFile(FilepathEventArgs e)
        {
            EventHandler<FilepathEventArgs> handler = StartingFile;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<StatusEventArgs> UpdateStatus;

        protected virtual void OnUpdateStatus(StatusEventArgs e)
        {
            EventHandler<StatusEventArgs> handler = UpdateStatus;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler ReportTaskWithoutProgress;

        protected virtual void OnReportTaskWithoutProgress(EventArgs e)
        {
            EventHandler handler = ReportTaskWithoutProgress;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler ReportTaskWithProgress;

        protected virtual void OnReportTaskWithProgress(EventArgs e)
        {
            EventHandler handler = ReportTaskWithProgress;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<ProgressEventArgs> UpdateProgress;

        protected virtual void OnUpdateProgress(ProgressEventArgs e)
        {
            EventHandler<ProgressEventArgs> handler = UpdateProgress;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<ExceptionEventArgs> ThrowException;

        protected virtual void OnThrowException(ExceptionEventArgs e)
        {
            EventHandler<ExceptionEventArgs> handler = ThrowException;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<FilepathEventArgs> FinishedFile;

        protected virtual void OnFinishedFile(FilepathEventArgs e)
        {
            EventHandler<FilepathEventArgs> handler = FinishedFile;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler Finished;

        protected virtual void OnFinished(EventArgs e)
        {
            EventHandler handler = Finished;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        private void HandleReportTaskWithoutProgress(object sender, EventArgs e)
        {
            OnReportTaskWithoutProgress(e);
        }

        private void HandleReportTaskWithProgress(object sender, EventArgs e)
        {
            OnReportTaskWithProgress(e);
        }

        private void HandleUpdateProgress(object sender, ProgressEventArgs e)
        {
            OnUpdateProgress(e);
        }

        public void Search()
        {
            StreamWriter overall_log = null;
            StreamWriter summary = null;
            StreamWriter log = null;
            FileStream protein_fasta_database = null;

            try
            {
                DateTime overall_start = DateTime.Now;

                OnStarting(EventArgs.Empty);

                OnUpdateProgress(new ProgressEventArgs(0));

                TandemMassSpectra.ReportTaskWithoutProgress += new EventHandler(HandleReportTaskWithoutProgress);
                TandemMassSpectra.ReportTaskWithProgress += new EventHandler(HandleReportTaskWithProgress);
                TandemMassSpectra.UpdateProgress += new EventHandler<ProgressEventArgs>(HandleUpdateProgress);

                PeptideSpectrumMatch.SetPrecursorMassType(precursorMassType);
                AminoAcidPolymer.SetProductMassType(productMassType);

                protein_fasta_database = new FileStream(proteinFastaDatabaseFilepath, FileMode.Open, FileAccess.Read, FileShare.Read);

                int target_proteins;
                int decoy_proteins;
                int on_the_fly_decoy_proteins;
                int total_proteins = ProteinFastaReader.CountProteins(protein_fasta_database, onTheFlyDecoys, out target_proteins, out decoy_proteins, out on_the_fly_decoy_proteins);
                double decoys_over_targets_protein_ratio = (double)(decoy_proteins + on_the_fly_decoy_proteins) / target_proteins;

                int num_target_peptides = 0;
                int num_decoy_peptides = 0;
                double decoys_over_targets_peptide_ratio = double.NaN;

                string fixed_modifications = null;
                foreach(Modification fixed_modification in fixedModifications)
                {
                    fixed_modifications += fixed_modification.ToString() + ", ";
                }
                if(fixed_modifications != null)
                {
                    fixed_modifications = fixed_modifications.Substring(0, fixed_modifications.Length - 2);
                }
                else
                {
                    fixed_modifications = "none";
                }
                string variable_modifications = null;
                foreach(Modification variable_modification in variableModifications)
                {
                    variable_modifications += variable_modification.ToString() + ", ";
                }
                if(variable_modifications != null)
                {
                    variable_modifications = variable_modifications.Substring(0, variable_modifications.Length - 2);
                }
                else
                {
                    variable_modifications = "none";
                }

                int total_spectra = 0;
                List<PeptideSpectrumMatch> aggregate_psms = null;

                SortedList<string, HashSet<string>> parents = null;
                Dictionary<string, int> num_spectra = null;
                Dictionary<string, List<PeptideSpectrumMatch>> grouped_aggregate_psms = null;

                if(dataFilepaths.Count > 1)
                {
                    overall_log = new StreamWriter(Path.Combine(outputFolder, "log.txt"));
                    overall_log.AutoFlush = true;

                    overall_log.WriteLine("MORPHEUS LOG");
                    overall_log.WriteLine();

                    overall_log.WriteLine("PARAMETERS");
                    string data_filepaths = null;
                    foreach(string data_filepath in dataFilepaths)
                    {
                        data_filepaths += data_filepath.ToString() + ", ";
                    }
                    data_filepaths = data_filepaths.Substring(0, data_filepaths.Length - 2);
                    overall_log.WriteLine("Input Data Files: " + data_filepaths);
                    overall_log.WriteLine("Unknown Precursor Charge State Range: " + minimumAssumedPrecursorChargeState.ToString("+0;-0;0") + ".." + maximumAssumedPrecursorChargeState.ToString("+0;-0;0"));
                    overall_log.WriteLine("Absolute MS/MS Intensity Threshold: " + (absoluteThreshold >= 0.0 ? absoluteThreshold.ToString() : "disabled"));
                    overall_log.WriteLine("Relative MS/MS Intensity Threshold: " + (relativeThresholdPercent >= 0.0 ? relativeThresholdPercent.ToString() + '%' : "disabled"));
                    overall_log.WriteLine("Maximum Number of MS/MS Peaks: " + (maximumNumberOfPeaks >= 0 ? maximumNumberOfPeaks.ToString() : "disabled"));
                    overall_log.WriteLine("Assign Charge States: " + assignChargeStates.ToString().ToLower());
                    overall_log.WriteLine("De-isotope: " + deisotope.ToString().ToLower());
                    overall_log.WriteLine("Protein FASTA Database: " + proteinFastaDatabaseFilepath);
                    overall_log.WriteLine("Create Target–Decoy Database On The Fly: " + onTheFlyDecoys.ToString().ToLower());
                    overall_log.WriteLine("Protease: " + protease.ToString());
                    overall_log.WriteLine("Maximum Missed Cleavages: " + maximumMissedCleavages.ToString());
                    overall_log.WriteLine("Initiator Methionine Behavior: " + initiatorMethionineBehavior.ToString().ToLower());
                    overall_log.WriteLine("Fixed Modifications: " + fixed_modifications);
                    overall_log.WriteLine("Variable Modifications: " + variable_modifications);
                    overall_log.WriteLine("Maximum Variable Modification Isoforms Per Peptide: " + maximumVariableModificationIsoforms.ToString());
                    overall_log.WriteLine("Precursor Mass Tolerance: ±" + precursorMassTolerance.Value.ToString() + ' ' + precursorMassTolerance.Units.ToString() + " (" + precursorMassType.ToString().ToLower() + ')');
                    overall_log.WriteLine("Precursor Monoisotopic Peak Correction: " + (precursorMonoisotopicPeakCorrection ? minimumPrecursorMonoisotopicPeakOffset.ToString("+0;-0;0") + ".." + maximumPrecursorMonoisotopicPeakOffset.ToString("+0;-0;0") : "disabled"));
                    overall_log.WriteLine("Product Mass Tolerance: ±" + productMassTolerance.Value.ToString() + ' ' + productMassTolerance.Units.ToString() + " (" + productMassType.ToString().ToLower() + ')');
                    overall_log.WriteLine("Maximum False Discovery Rate: " + (maximumFalseDiscoveryRate * 100).ToString() + '%');
                    overall_log.WriteLine("Consider Modified Forms as Unique Peptides: " + considerModifiedFormsAsUniquePeptides.ToString().ToLower());
                    overall_log.WriteLine("Maximum Threads: " + maximumThreads.ToString());
                    overall_log.WriteLine("Minimize Memory Usage: " + minimizeMemoryUsage.ToString().ToLower());
                    overall_log.WriteLine("Output Folder: " + outputFolder.ToString());
                    overall_log.WriteLine();

                    overall_log.WriteLine("RESULTS");

                    overall_log.WriteLine(total_proteins.ToString("N0") + " total (" + target_proteins.ToString("N0") + " target + " + decoy_proteins.ToString("N0") + " decoy + " + on_the_fly_decoy_proteins.ToString("N0") + " on-the-fly decoy) proteins");

                    aggregate_psms = new List<PeptideSpectrumMatch>();

                    parents = DetermineSemiAggregateParentFolders(dataFilepaths);
                    if(parents.Count > 0)
                    {
                        num_spectra = new Dictionary<string, int>(dataFilepaths.Count);
                        grouped_aggregate_psms = new Dictionary<string, List<PeptideSpectrumMatch>>(dataFilepaths.Count);
                    }
                }

                summary = new StreamWriter(Path.Combine(outputFolder, "summary.tsv"));
                summary.AutoFlush = true;

                summary.WriteLine("Dataset\tProteins\tMS/MS Spectra\tPSM Morpheus Score Threshold\tTarget PSMs\tDecoy PSMs\tPSM FDR (%)\tUnique Peptide Morpheus Score Threshold\tUnique Target Peptides\tUnique Decoy Peptides\tUnique Peptide FDR (%)\tProtein Group Summed Morpheus Score Threshold\tTarget Protein Groups\tDecoy Protein Groups\tProtein Group FDR (%)");

                foreach(string data_filepath in dataFilepaths)
                {
                    DateTime start = DateTime.Now;

                    OnStartingFile(new FilepathEventArgs(data_filepath));

                    OnUpdateProgress(new ProgressEventArgs(0));

                    log = new StreamWriter(Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(data_filepath) + ".log.txt"));
                    log.AutoFlush = true;

                    log.WriteLine("MORPHEUS LOG");
                    log.WriteLine();

                    log.WriteLine("PARAMETERS");
                    log.WriteLine("Input Data File: " + data_filepath);
                    log.WriteLine("Unknown Precursor Charge State Range: " + minimumAssumedPrecursorChargeState.ToString("+0;-0;0") + ".." + maximumAssumedPrecursorChargeState.ToString("+0;-0;0"));
                    log.WriteLine("Absolute MS/MS Intensity Threshold: " + (absoluteThreshold >= 0.0 ? absoluteThreshold.ToString() : "disabled"));
                    log.WriteLine("Relative MS/MS Intensity Threshold: " + (relativeThresholdPercent >= 0.0 ? relativeThresholdPercent.ToString() + '%' : "disabled"));
                    log.WriteLine("Maximum Number of MS/MS Peaks: " + (maximumNumberOfPeaks >= 0 ? maximumNumberOfPeaks.ToString() : "disabled"));
                    log.WriteLine("Assign Charge States: " + assignChargeStates.ToString().ToLower());
                    log.WriteLine("De-isotope: " + deisotope.ToString().ToLower());
                    log.WriteLine("Protein FASTA Database: " + proteinFastaDatabaseFilepath);
                    log.WriteLine("Create Target–Decoy Database On The Fly: " + onTheFlyDecoys.ToString().ToLower());
                    log.WriteLine("Protease: " + protease.ToString());
                    log.WriteLine("Maximum Missed Cleavages: " + maximumMissedCleavages.ToString());
                    log.WriteLine("Initiator Methionine Behavior: " + initiatorMethionineBehavior.ToString().ToLower());
                    log.WriteLine("Fixed Modifications: " + fixed_modifications);
                    log.WriteLine("Variable Modifications: " + variable_modifications);
                    log.WriteLine("Maximum Variable Modification Isoforms Per Peptide: " + maximumVariableModificationIsoforms.ToString());
                    log.WriteLine("Precursor Mass Tolerance: ±" + precursorMassTolerance.Value.ToString() + ' ' + precursorMassTolerance.Units.ToString() + " (" + precursorMassType.ToString().ToLower() + ')');
                    log.WriteLine("Precursor Monoisotopic Peak Correction: " + (precursorMonoisotopicPeakCorrection ? minimumPrecursorMonoisotopicPeakOffset.ToString("+0;-0;0") + ".." + maximumPrecursorMonoisotopicPeakOffset.ToString("+0;-0;0") : "disabled"));
                    log.WriteLine("Product Mass Tolerance: ±" + productMassTolerance.Value.ToString() + ' ' + productMassTolerance.Units.ToString() + " (" + productMassType.ToString().ToLower() + ')');
                    log.WriteLine("Maximum False Discovery Rate: " + (maximumFalseDiscoveryRate * 100).ToString() + '%');
                    log.WriteLine("Consider Modified Forms as Unique Peptides: " + considerModifiedFormsAsUniquePeptides.ToString().ToLower());
                    log.WriteLine("Maximum Threads: " + maximumThreads.ToString());
                    log.WriteLine("Minimize Memory Usage: " + minimizeMemoryUsage.ToString().ToLower());
                    log.WriteLine("Output Folder: " + outputFolder.ToString());
                    log.WriteLine();

                    log.WriteLine("RESULTS");

                    log.WriteLine(total_proteins.ToString("N0") + " total (" + target_proteins.ToString("N0") + " target + " + decoy_proteins.ToString("N0") + " decoy + " + on_the_fly_decoy_proteins.ToString("N0") + " on-the-fly decoy) proteins");

                    OnUpdateStatus(new StatusEventArgs("Extracting and preprocessing MS/MS spectra..."));
                    OnReportTaskWithProgress(EventArgs.Empty);
                    OnUpdateProgress(new ProgressEventArgs(0));

                    TandemMassSpectra spectra = TandemMassSpectra.Load(data_filepath, minimumAssumedPrecursorChargeState, maximumAssumedPrecursorChargeState,
                        absoluteThreshold, relativeThresholdPercent, maximumNumberOfPeaks,
                        assignChargeStates, deisotope, productMassTolerance, maximumThreads);

                    if(dataFilepaths.Count > 1)
                    {
                        total_spectra += spectra.Count;
                        if(parents.Count > 0)
                        {
                            num_spectra.Add(data_filepath, spectra.Count);
                        }
                    }

                    OnUpdateStatus(new StatusEventArgs("Searching MS/MS spectra..."));
                    OnReportTaskWithProgress(EventArgs.Empty);
                    OnUpdateProgress(new ProgressEventArgs(0));

                    PeptideSpectrumMatch[] psms = null;
                    if(spectra.Count > 0)
                    {
                        int max_spectrum_number = 0;
                        foreach(TandemMassSpectrum spectrum in spectra)
                        {
                            if(spectrum.SpectrumNumber > max_spectrum_number)
                            {
                                max_spectrum_number = spectrum.SpectrumNumber;
                            }
                        }

                        psms = new PeptideSpectrumMatch[max_spectrum_number];

                        spectra.Sort(TandemMassSpectrum.AscendingPrecursorMassComparison);
                    }

                    Dictionary<string, bool> peptides_observed = null;
                    if(!minimizeMemoryUsage)
                    {
                        peptides_observed = new Dictionary<string, bool>();
                    }

                    num_target_peptides = 0;
                    num_decoy_peptides = 0;

#if NON_MULTITHREADED
                    int proteins = 0;
                    int old_progress = 0;
                    foreach(Protein protein in ProteinFastaReader.ReadProteins(protein_fasta_database, onTheFlyDecoys))
                    {
                        foreach(Peptide peptide in protein.Digest(protease, maximumMissedCleavages, initiatorMethionineBehavior, null, null))
                        {
                            if(peptide.Target)
                            {
                                num_target_peptides++;
                            }
                            else
                            {
                                num_decoy_peptides++;
                            }

                            if(!minimizeMemoryUsage)
                            {
                                // This block of code is to ensure that (1) we don't re-search the same base leucine peptide sequence more than we need to, 
                                // and (2) that we are maximally conservative by calling PSMs decoy whenever possible.
                                // If we haven't already seen this base leucine peptide sequence, add it to the dictionary with a value indicating whether it was decoy or not.
                                // Then perform the search as usual.
                                // If we have already seen it and it was decoy or this time it is target, we don't need to search it again, skip the peptide.
                                // Otherwise, update the dictionary to reflect that we have now seen it as a decoy and perform the search.
                                bool observed_as_decoy = false;
                                if(!peptides_observed.TryGetValue(peptide.BaseLeucineSequence, out observed_as_decoy))
                                {
                                    peptides_observed.Add(peptide.BaseLeucineSequence, peptide.Decoy);
                                }
                                else
                                {
                                    if(observed_as_decoy || peptide.Target)
                                    {
                                        continue;
                                    }

                                    peptides_observed[peptide.BaseLeucineSequence] = true;
                                }
                            }

                            peptide.SetFixedModifications(fixedModifications);
                            foreach(Peptide modified_peptide in peptide.GetVariablyModifiedPeptides(variableModifications, maximumVariableModificationIsoforms))
                            {
                                foreach(TandemMassSpectrum spectrum in precursorMonoisotopicPeakCorrection ?
                                    spectra.GetTandemMassSpectraInMassRange(precursorMassType == MassType.Average ? modified_peptide.AverageMass : modified_peptide.MonoisotopicMass, precursorMassTolerance, minimumPrecursorMonoisotopicPeakOffset, maximumPrecursorMonoisotopicPeakOffset) :
                                    spectra.GetTandemMassSpectraInMassRange(precursorMassType == MassType.Average ? modified_peptide.AverageMass : modified_peptide.MonoisotopicMass, precursorMassTolerance))
                                {
                                    PeptideSpectrumMatch psm = new PeptideSpectrumMatch(spectrum, modified_peptide, productMassTolerance);
                                    PeptideSpectrumMatch current_best_psm = psms[spectrum.SpectrumNumber - 1];
                                    if(current_best_psm == null || PeptideSpectrumMatch.DescendingMorpheusScoreComparison(psm, current_best_psm) < 0)
                                    {
                                        psms[spectrum.SpectrumNumber - 1] = psm;
                                    }
                                }
                            }
                        }

                        proteins++;
                        int new_progress = (int)((double)proteins / total_proteins * 100);
                        if(new_progress > old_progress)
                        {
                            OnUpdateProgress(new ProgressEventArgs(new_progress));
                            old_progress = new_progress;
                        }
                    }
#else
                    object progress_lock = new object();
                    int proteins = 0;
                    int old_progress = 0;
                    ParallelOptions parallel_options = new ParallelOptions();
                    parallel_options.MaxDegreeOfParallelism = maximumThreads;
                    Parallel.ForEach(ProteinFastaReader.ReadProteins(protein_fasta_database, onTheFlyDecoys), parallel_options, protein =>
                        {
                            foreach(Peptide peptide in protein.Digest(protease, maximumMissedCleavages, initiatorMethionineBehavior, null, null))
                            {
                                if(peptide.Target)
                                {
                                    Interlocked.Increment(ref num_target_peptides);
                                }
                                else
                                {
                                    Interlocked.Increment(ref num_decoy_peptides);
                                }

                                if(!minimizeMemoryUsage)
                                {
                                    // This block of code is to ensure that (1) we don't re-search the same base leucine peptide sequence more than we need to, 
                                    // and (2) that we are maximally conservative by calling PSMs decoy whenever possible.
                                    // If we haven't already seen this base leucine peptide sequence, add it to the dictionary with a value indicating whether it was decoy or not.
                                    // Then perform the search as usual.
                                    // If we have already seen it and it was decoy or this time it is target, we don't need to search it again, skip the peptide.
                                    // Otherwise, update the dictionary to reflect that we have now seen it as a decoy and perform the search.
                                    lock(peptides_observed)
                                    {
                                        bool observed_as_decoy = false;
                                        if(!peptides_observed.TryGetValue(peptide.BaseLeucineSequence, out observed_as_decoy))
                                        {
                                            peptides_observed.Add(peptide.BaseLeucineSequence, peptide.Decoy);
                                        }
                                        else
                                        {
                                            if(observed_as_decoy || peptide.Target)
                                            {
                                                continue;
                                            }

                                            peptides_observed[peptide.BaseLeucineSequence] = true;
                                        }
                                    }
                                }

                                peptide.SetFixedModifications(fixedModifications);
                                foreach(Peptide modified_peptide in peptide.GetVariablyModifiedPeptides(variableModifications, maximumVariableModificationIsoforms))
                                {
                                    foreach(TandemMassSpectrum spectrum in precursorMonoisotopicPeakCorrection ?
                                        spectra.GetTandemMassSpectraInMassRange(precursorMassType == MassType.Average ? modified_peptide.AverageMass : modified_peptide.MonoisotopicMass, precursorMassTolerance, minimumPrecursorMonoisotopicPeakOffset, maximumPrecursorMonoisotopicPeakOffset) :
                                        spectra.GetTandemMassSpectraInMassRange(precursorMassType == MassType.Average ? modified_peptide.AverageMass : modified_peptide.MonoisotopicMass, precursorMassTolerance))
                                    {
                                        PeptideSpectrumMatch psm = new PeptideSpectrumMatch(spectrum, modified_peptide, productMassTolerance);
                                        lock(psms)
                                        {
                                            PeptideSpectrumMatch current_best_psm = psms[spectrum.SpectrumNumber - 1];
                                            if(current_best_psm == null || PeptideSpectrumMatch.DescendingMorpheusScoreComparison(psm, current_best_psm) < 0)
                                            {
                                                psms[spectrum.SpectrumNumber - 1] = psm;
                                            }
                                        }
                                    }
                                }
                            }

                            lock(progress_lock)
                            {
                                proteins++;
                                int new_progress = (int)((double)proteins / total_proteins * 100);
                                if(new_progress > old_progress)
                                {
                                    OnUpdateProgress(new ProgressEventArgs(new_progress));
                                    old_progress = new_progress;
                                }
                            }
                        }
                    );
#endif

                    OnUpdateStatus(new StatusEventArgs("Performing post-search analyses..."));
                    OnReportTaskWithoutProgress(EventArgs.Empty);
                    OnUpdateProgress(new ProgressEventArgs(0));

                    log.WriteLine((num_target_peptides + num_decoy_peptides).ToString("N0") + " total (" + num_target_peptides.ToString("N0") + " target + " + num_decoy_peptides.ToString("N0") + " decoy) non-unique peptides");
                    decoys_over_targets_peptide_ratio = (double)num_decoy_peptides / num_target_peptides;

                    log.WriteLine(spectra.Count.ToString("N0") + " MS/MS spectra");

                    List<PeptideSpectrumMatch> psms_no_nulls;
                    if(psms != null)
                    {
                        psms_no_nulls = new List<PeptideSpectrumMatch>(psms.Length);
                        foreach(PeptideSpectrumMatch psm in psms)
                        {
                            if(psm != null)
                            {
                                psms_no_nulls.Add(psm);
                            }
                        }

                        if(dataFilepaths.Count > 1)
                        {
                            aggregate_psms.AddRange(psms_no_nulls);
                            if(parents.Count > 0)
                            {
                                grouped_aggregate_psms.Add(data_filepath, psms_no_nulls);
                            }
                        }
                    }
                    else
                    {
                        psms_no_nulls = new List<PeptideSpectrumMatch>(0);
                    }

                    List<PeptideSpectrumMatch> sorted_psms = new List<PeptideSpectrumMatch>(psms_no_nulls);
                    sorted_psms.Sort(PeptideSpectrumMatch.DescendingMorpheusScoreComparison);

                    IEnumerable<IdentificationWithFalseDiscoveryRate<PeptideSpectrumMatch>> psms_with_fdr = FalseDiscoveryRate.DoFalseDiscoveryRateAnalysis(sorted_psms, decoys_over_targets_peptide_ratio);
                    Exporters.WriteToTabDelimitedTextFile(psms_with_fdr, Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(data_filepath) + ".PSMs.tsv"));
                    double psm_score_threshold = double.NegativeInfinity;
                    int target_psms = sorted_psms.Count;
                    int decoy_psms = 0;
                    double psm_fdr = double.NaN;
                    if(decoys_over_targets_peptide_ratio == 0.0)
                    {
                        log.WriteLine(sorted_psms.Count.ToString("N0") + " PSMs (unknown FDR)");
                    }
                    else
                    {
                        FalseDiscoveryRate.DetermineMaximumIdentifications(psms_with_fdr, false, maximumFalseDiscoveryRate, out psm_score_threshold, out target_psms, out decoy_psms, out psm_fdr);
                        log.WriteLine(target_psms.ToString("N0") + " target (" + decoy_psms.ToString("N0") + " decoy) PSMs at " + psm_fdr.ToString("0.000%") + " PSM FDR (" + psm_score_threshold.ToString("0.000") + " Morpheus score threshold)");
                    }

                    Exporters.WritePsmsToPepXmlFile(Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(data_filepath) + ".pep.xml"),
                        data_filepath,
                        minimumAssumedPrecursorChargeState, maximumAssumedPrecursorChargeState,
                        absoluteThreshold, relativeThresholdPercent, maximumNumberOfPeaks,
                        assignChargeStates, deisotope,
                        proteinFastaDatabaseFilepath, onTheFlyDecoys, onTheFlyDecoys ? proteins / 2 : proteins,
                        protease, maximumMissedCleavages, initiatorMethionineBehavior,
                        fixedModifications, fixed_modifications, variableModifications, variable_modifications, maximumVariableModificationIsoforms,
                        precursorMassTolerance, precursorMassType,
                        precursorMonoisotopicPeakCorrection, minimumPrecursorMonoisotopicPeakOffset, maximumPrecursorMonoisotopicPeakOffset,
                        productMassTolerance, productMassType,
                        maximumFalseDiscoveryRate, considerModifiedFormsAsUniquePeptides,
                        maximumThreads, minimizeMemoryUsage,
                        outputFolder,
                        psms_with_fdr);

                    Dictionary<string, PeptideSpectrumMatch> peptides = new Dictionary<string, PeptideSpectrumMatch>();

                    foreach(PeptideSpectrumMatch psm in sorted_psms)
                    {
                        if(!peptides.ContainsKey(considerModifiedFormsAsUniquePeptides ? psm.Peptide.LeucineSequence : psm.Peptide.BaseLeucineSequence))
                        {
                            peptides.Add(considerModifiedFormsAsUniquePeptides ? psm.Peptide.LeucineSequence : psm.Peptide.BaseLeucineSequence, psm);
                        }
                    }

                    List<PeptideSpectrumMatch> sorted_peptides = new List<PeptideSpectrumMatch>(peptides.Values);
                    sorted_peptides.Sort(PeptideSpectrumMatch.DescendingMorpheusScoreComparison);

                    IEnumerable<IdentificationWithFalseDiscoveryRate<PeptideSpectrumMatch>> peptides_with_fdr = FalseDiscoveryRate.DoFalseDiscoveryRateAnalysis(sorted_peptides, decoys_over_targets_peptide_ratio);
                    Exporters.WriteToTabDelimitedTextFile(peptides_with_fdr, Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(data_filepath) + ".unique_peptides.tsv"));
                    double peptide_score_threshold = double.NegativeInfinity;
                    int target_peptides = sorted_peptides.Count;
                    int decoy_peptides = 0;
                    double peptide_fdr = double.NaN;
                    if(decoys_over_targets_peptide_ratio == 0.0)
                    {
                        log.WriteLine(sorted_peptides.Count.ToString("N0") + " unique peptides (unknown FDR)");
                    }
                    else
                    {
                        FalseDiscoveryRate.DetermineMaximumIdentifications(peptides_with_fdr, false, maximumFalseDiscoveryRate, out peptide_score_threshold, out target_peptides, out decoy_peptides, out peptide_fdr);
                        log.WriteLine(target_peptides.ToString("N0") + " unique target (" + decoy_peptides.ToString("N0") + " decoy) peptides at " + peptide_fdr.ToString("0.000%") + " unique peptide FDR (" + peptide_score_threshold.ToString("0.000") + " Morpheus score threshold)");
                    }

                    List<ProteinGroup> protein_groups = ProteinGroup.ApplyProteinParsimony(sorted_psms, peptide_score_threshold, protein_fasta_database, onTheFlyDecoys, protease, maximumMissedCleavages, initiatorMethionineBehavior, maximumThreads);

                    IEnumerable<IdentificationWithFalseDiscoveryRate<ProteinGroup>> protein_groups_with_fdr = FalseDiscoveryRate.DoFalseDiscoveryRateAnalysis(protein_groups, decoys_over_targets_protein_ratio);
                    Exporters.WriteToTabDelimitedTextFile(protein_groups_with_fdr, Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(data_filepath) + ".protein_groups.tsv"));
                    double protein_group_score_threshold = double.NegativeInfinity;
                    int target_protein_groups = protein_groups.Count;
                    int decoy_protein_groups = 0;
                    double protein_group_fdr = double.NaN;
                    if(decoys_over_targets_protein_ratio == 0.0)
                    {
                        log.WriteLine(protein_groups.Count.ToString("N0") + " protein groups (unknown FDR)");
                    }
                    else
                    {
                        FalseDiscoveryRate.DetermineMaximumIdentifications(protein_groups_with_fdr, false, maximumFalseDiscoveryRate, out protein_group_score_threshold, out target_protein_groups, out decoy_protein_groups, out protein_group_fdr);
                        log.WriteLine(target_protein_groups.ToString("N0") + " target (" + decoy_protein_groups.ToString("N0") + " decoy) protein groups at " + protein_group_fdr.ToString("0.000%") + " protein group FDR (" + protein_group_score_threshold.ToString("0.000") + " summed Morpheus score threshold)");
                    }

                    DateTime stop = DateTime.Now;
                    log.WriteLine((stop - start).TotalMinutes.ToString("0.00") + " minutes to analyze");

                    log.Close();

                    summary.Write(data_filepath + '\t');
                    summary.Write(proteins.ToString() + '\t');
                    summary.Write(spectra.Count.ToString() + '\t');
                    summary.Write(psm_score_threshold.ToString("0.000") + '\t');
                    summary.Write(target_psms.ToString() + '\t');
                    summary.Write(decoy_psms.ToString() + '\t');
                    summary.Write(psm_fdr.ToString("0.000%") + '\t');
                    summary.Write(peptide_score_threshold.ToString("0.000") + '\t');
                    summary.Write(target_peptides.ToString() + '\t');
                    summary.Write(decoy_peptides.ToString() + '\t');
                    summary.Write(peptide_fdr.ToString("0.000%") + '\t');
                    summary.Write(protein_group_score_threshold.ToString("0.000") + '\t');
                    summary.Write(target_protein_groups.ToString() + '\t');
                    summary.Write(decoy_protein_groups.ToString() + '\t');
                    summary.Write(protein_group_fdr.ToString("0.000%") + '\t');
                    summary.WriteLine();

                    OnFinishedFile(new FilepathEventArgs(data_filepath));
                }

                overall_log.WriteLine((num_target_peptides + num_decoy_peptides).ToString("N0") + " total (" + num_target_peptides.ToString("N0") + " target + " + num_decoy_peptides.ToString("N0") + " decoy) non-unique peptides");

                if(dataFilepaths.Count > 1)
                {
                    OnUpdateStatus(new StatusEventArgs("Performing aggregate post-search analyses..."));
                    OnReportTaskWithoutProgress(EventArgs.Empty);
                    OnUpdateProgress(new ProgressEventArgs(0));

                    HashSet<string> prefixes = new HashSet<string>();
                    foreach(KeyValuePair<string, HashSet<string>> kvp in parents)
                    {
                        DirectoryInfo directory_info = new DirectoryInfo(kvp.Key);
                        string prefix = directory_info.Name;
                        int id = 1;
                        while(prefixes.Contains(prefix))
                        {
                            id++;
                            prefix = directory_info.Name + '#' + id.ToString();
                        }

                        int semi_aggregate_spectra = 0;
                        List<PeptideSpectrumMatch> semi_aggregate_psms = new List<PeptideSpectrumMatch>();
                        foreach(string data_filepath in kvp.Value)
                        {
                            semi_aggregate_spectra += num_spectra[data_filepath];
                            semi_aggregate_psms.AddRange(grouped_aggregate_psms[data_filepath]);
                        }

                        overall_log.WriteLine(semi_aggregate_spectra.ToString("N0") + " MS/MS spectra in " + kvp.Key);

                        semi_aggregate_psms.Sort(PeptideSpectrumMatch.DescendingMorpheusScoreComparison);

                        IEnumerable<IdentificationWithFalseDiscoveryRate<PeptideSpectrumMatch>> semi_aggregate_psms_with_fdr = FalseDiscoveryRate.DoFalseDiscoveryRateAnalysis(semi_aggregate_psms, decoys_over_targets_peptide_ratio);
                        Exporters.WriteToTabDelimitedTextFile(semi_aggregate_psms_with_fdr, Path.Combine(outputFolder, prefix + ".PSMs.tsv"));
                        double semi_aggregate_psm_score_threshold;
                        int semi_aggregate_target_psms;
                        int semi_aggregate_decoy_psms;
                        double semi_aggregate_psm_fdr;
                        FalseDiscoveryRate.DetermineMaximumIdentifications(semi_aggregate_psms_with_fdr, false, maximumFalseDiscoveryRate, out semi_aggregate_psm_score_threshold, out semi_aggregate_target_psms, out semi_aggregate_decoy_psms, out semi_aggregate_psm_fdr);
                        overall_log.WriteLine(semi_aggregate_target_psms.ToString("N0") + " target (" + semi_aggregate_decoy_psms.ToString("N0") + " decoy) PSMs at " + semi_aggregate_psm_fdr.ToString("0.000%") + " PSM FDR (" + semi_aggregate_psm_score_threshold.ToString("0.000") + " Morpheus score threshold) in " + kvp.Key);

                        Dictionary<string, PeptideSpectrumMatch> semi_aggregate_peptides = new Dictionary<string, PeptideSpectrumMatch>();

                        foreach(PeptideSpectrumMatch psm in semi_aggregate_psms)
                        {
                            if(!semi_aggregate_peptides.ContainsKey(considerModifiedFormsAsUniquePeptides ? psm.Peptide.LeucineSequence : psm.Peptide.BaseLeucineSequence))
                            {
                                semi_aggregate_peptides.Add(considerModifiedFormsAsUniquePeptides ? psm.Peptide.LeucineSequence : psm.Peptide.BaseLeucineSequence, psm);
                            }
                        }

                        List<PeptideSpectrumMatch> semi_aggregate_sorted_peptides = new List<PeptideSpectrumMatch>(semi_aggregate_peptides.Values);
                        semi_aggregate_sorted_peptides.Sort(PeptideSpectrumMatch.DescendingMorpheusScoreComparison);

                        IEnumerable<IdentificationWithFalseDiscoveryRate<PeptideSpectrumMatch>> semi_aggregate_peptides_with_fdr = FalseDiscoveryRate.DoFalseDiscoveryRateAnalysis(semi_aggregate_sorted_peptides, decoys_over_targets_peptide_ratio);
                        Exporters.WriteToTabDelimitedTextFile(semi_aggregate_peptides_with_fdr, Path.Combine(outputFolder, prefix + ".unique_peptides.tsv"));
                        double semi_aggregate_peptide_score_threshold;
                        int semi_aggregate_target_peptides;
                        int semi_aggregate_decoy_peptides;
                        double semi_aggregate_peptide_fdr;
                        FalseDiscoveryRate.DetermineMaximumIdentifications(semi_aggregate_peptides_with_fdr, false, maximumFalseDiscoveryRate, out semi_aggregate_peptide_score_threshold, out semi_aggregate_target_peptides, out semi_aggregate_decoy_peptides, out semi_aggregate_peptide_fdr);
                        overall_log.WriteLine(semi_aggregate_target_peptides.ToString("N0") + " unique target (" + semi_aggregate_decoy_peptides.ToString("N0") + " decoy) peptides at " + semi_aggregate_peptide_fdr.ToString("0.000%") + " unique peptide FDR (" + semi_aggregate_peptide_score_threshold.ToString("0.000") + " Morpheus score threshold) in " + kvp.Key);

                        List<ProteinGroup> semi_aggregate_protein_groups = ProteinGroup.ApplyProteinParsimony(semi_aggregate_psms, semi_aggregate_peptide_score_threshold, protein_fasta_database, onTheFlyDecoys, protease, maximumMissedCleavages, initiatorMethionineBehavior, maximumThreads);

                        IEnumerable<IdentificationWithFalseDiscoveryRate<ProteinGroup>> semi_aggregate_protein_groups_with_fdr = FalseDiscoveryRate.DoFalseDiscoveryRateAnalysis(semi_aggregate_protein_groups, decoys_over_targets_protein_ratio);
                        Exporters.WriteToTabDelimitedTextFile(semi_aggregate_protein_groups_with_fdr, Path.Combine(outputFolder, prefix + ".protein_groups.tsv"));
                        double semi_aggregate_protein_group_score_threshold;
                        int semi_aggregate_target_protein_groups;
                        int semi_aggregate_decoy_protein_groups;
                        double semi_aggregate_protein_group_fdr;
                        FalseDiscoveryRate.DetermineMaximumIdentifications(semi_aggregate_protein_groups_with_fdr, false, maximumFalseDiscoveryRate, out semi_aggregate_protein_group_score_threshold, out semi_aggregate_target_protein_groups, out semi_aggregate_decoy_protein_groups, out semi_aggregate_protein_group_fdr);
                        overall_log.WriteLine(semi_aggregate_target_protein_groups.ToString("N0") + " target (" + semi_aggregate_decoy_protein_groups.ToString("N0") + " decoy) protein groups at " + semi_aggregate_protein_group_fdr.ToString("0.000%") + " protein group FDR (" + semi_aggregate_protein_group_score_threshold.ToString("0.000") + " summed Morpheus score threshold) in " + kvp.Key);

                        summary.Write(kvp.Key + '\t');
                        summary.Write(total_proteins.ToString() + '\t');
                        summary.Write(semi_aggregate_spectra.ToString() + '\t');
                        summary.Write(semi_aggregate_psm_score_threshold.ToString("0.000") + '\t');
                        summary.Write(semi_aggregate_target_psms.ToString() + '\t');
                        summary.Write(semi_aggregate_decoy_psms.ToString() + '\t');
                        summary.Write(semi_aggregate_psm_fdr.ToString("0.000%") + '\t');
                        summary.Write(semi_aggregate_peptide_score_threshold.ToString("0.000") + '\t');
                        summary.Write(semi_aggregate_target_peptides.ToString() + '\t');
                        summary.Write(semi_aggregate_decoy_peptides.ToString() + '\t');
                        summary.Write(semi_aggregate_peptide_fdr.ToString("0.000%") + '\t');
                        summary.Write(semi_aggregate_protein_group_score_threshold.ToString("0.000") + '\t');
                        summary.Write(semi_aggregate_target_protein_groups.ToString() + '\t');
                        summary.Write(semi_aggregate_decoy_protein_groups.ToString() + '\t');
                        summary.Write(semi_aggregate_protein_group_fdr.ToString("0.000%") + '\t');
                        summary.WriteLine();
                    }

                    overall_log.WriteLine(total_spectra.ToString("N0") + " MS/MS spectra");

                    aggregate_psms.Sort(PeptideSpectrumMatch.DescendingMorpheusScoreComparison);

                    IEnumerable<IdentificationWithFalseDiscoveryRate<PeptideSpectrumMatch>> aggregate_psms_with_fdr = FalseDiscoveryRate.DoFalseDiscoveryRateAnalysis(aggregate_psms, decoys_over_targets_peptide_ratio);
                    Exporters.WriteToTabDelimitedTextFile(aggregate_psms_with_fdr, Path.Combine(outputFolder, "PSMs.tsv"));
                    double aggregate_psm_score_threshold;
                    int aggregate_target_psms;
                    int aggregate_decoy_psms;
                    double aggregate_psm_fdr;
                    FalseDiscoveryRate.DetermineMaximumIdentifications(aggregate_psms_with_fdr, false, maximumFalseDiscoveryRate, out aggregate_psm_score_threshold, out aggregate_target_psms, out aggregate_decoy_psms, out aggregate_psm_fdr);
                    overall_log.WriteLine(aggregate_target_psms.ToString("N0") + " target (" + aggregate_decoy_psms.ToString("N0") + " decoy) aggregate PSMs at " + aggregate_psm_fdr.ToString("0.000%") + " PSM FDR (" + aggregate_psm_score_threshold.ToString("0.000") + " Morpheus score threshold)");

                    Dictionary<string, PeptideSpectrumMatch> aggregate_peptides = new Dictionary<string, PeptideSpectrumMatch>();

                    foreach(PeptideSpectrumMatch psm in aggregate_psms)
                    {
                        if(!aggregate_peptides.ContainsKey(considerModifiedFormsAsUniquePeptides ? psm.Peptide.LeucineSequence : psm.Peptide.BaseLeucineSequence))
                        {
                            aggregate_peptides.Add(considerModifiedFormsAsUniquePeptides ? psm.Peptide.LeucineSequence : psm.Peptide.BaseLeucineSequence, psm);
                        }
                    }

                    List<PeptideSpectrumMatch> aggregate_sorted_peptides = new List<PeptideSpectrumMatch>(aggregate_peptides.Values);
                    aggregate_sorted_peptides.Sort(PeptideSpectrumMatch.DescendingMorpheusScoreComparison);

                    IEnumerable<IdentificationWithFalseDiscoveryRate<PeptideSpectrumMatch>> aggregate_peptides_with_fdr = FalseDiscoveryRate.DoFalseDiscoveryRateAnalysis(aggregate_sorted_peptides, decoys_over_targets_peptide_ratio);
                    Exporters.WriteToTabDelimitedTextFile(aggregate_peptides_with_fdr, Path.Combine(outputFolder, "unique_peptides.tsv"));
                    double aggregate_peptide_score_threshold;
                    int aggregate_target_peptides;
                    int aggregate_decoy_peptides;
                    double aggregate_peptide_fdr;
                    FalseDiscoveryRate.DetermineMaximumIdentifications(aggregate_peptides_with_fdr, false, maximumFalseDiscoveryRate, out aggregate_peptide_score_threshold, out aggregate_target_peptides, out aggregate_decoy_peptides, out aggregate_peptide_fdr);
                    overall_log.WriteLine(aggregate_target_peptides.ToString("N0") + " unique target (" + aggregate_decoy_peptides.ToString("N0") + " decoy) aggregate peptides at " + aggregate_peptide_fdr.ToString("0.000%") + " unique peptide FDR (" + aggregate_peptide_score_threshold.ToString("0.000") + " Morpheus score threshold)");

                    List<ProteinGroup> aggregate_protein_groups = ProteinGroup.ApplyProteinParsimony(aggregate_psms, aggregate_peptide_score_threshold, protein_fasta_database, onTheFlyDecoys, protease, maximumMissedCleavages, initiatorMethionineBehavior, maximumThreads);

                    IEnumerable<IdentificationWithFalseDiscoveryRate<ProteinGroup>> aggregate_protein_groups_with_fdr = FalseDiscoveryRate.DoFalseDiscoveryRateAnalysis(aggregate_protein_groups, decoys_over_targets_protein_ratio);
                    Exporters.WriteToTabDelimitedTextFile(aggregate_protein_groups_with_fdr, Path.Combine(outputFolder, "protein_groups.tsv"));
                    double aggregate_protein_group_score_threshold;
                    int aggregate_target_protein_groups;
                    int aggregate_decoy_protein_groups;
                    double aggregate_protein_group_fdr;
                    FalseDiscoveryRate.DetermineMaximumIdentifications(aggregate_protein_groups_with_fdr, false, maximumFalseDiscoveryRate, out aggregate_protein_group_score_threshold, out aggregate_target_protein_groups, out aggregate_decoy_protein_groups, out aggregate_protein_group_fdr);
                    overall_log.WriteLine(aggregate_target_protein_groups.ToString("N0") + " target (" + aggregate_decoy_protein_groups.ToString("N0") + " decoy) aggregate protein groups at " + aggregate_protein_group_fdr.ToString("0.000%") + " protein group FDR (" + aggregate_protein_group_score_threshold.ToString("0.000") + " summed Morpheus score threshold)");

                    DateTime overall_stop = DateTime.Now;
                    overall_log.WriteLine((overall_stop - overall_start).TotalMinutes.ToString("0.00") + " minutes to analyze");

                    overall_log.Close();

                    summary.Write("AGGREGATE" + '\t');
                    summary.Write(total_proteins.ToString() + '\t');
                    summary.Write(total_spectra.ToString() + '\t');
                    summary.Write(aggregate_psm_score_threshold.ToString("0.000") + '\t');
                    summary.Write(aggregate_target_psms.ToString() + '\t');
                    summary.Write(aggregate_decoy_psms.ToString() + '\t');
                    summary.Write(aggregate_psm_fdr.ToString("0.000%") + '\t');
                    summary.Write(aggregate_peptide_score_threshold.ToString("0.000") + '\t');
                    summary.Write(aggregate_target_peptides.ToString() + '\t');
                    summary.Write(aggregate_decoy_peptides.ToString() + '\t');
                    summary.Write(aggregate_peptide_fdr.ToString("0.000%") + '\t');
                    summary.Write(aggregate_protein_group_score_threshold.ToString("0.000") + '\t');
                    summary.Write(aggregate_target_protein_groups.ToString() + '\t');
                    summary.Write(aggregate_decoy_protein_groups.ToString() + '\t');
                    summary.Write(aggregate_protein_group_fdr.ToString("0.000%") + '\t');
                    summary.WriteLine();
                }

                protein_fasta_database.Close();

                summary.Close();

                OnFinished(EventArgs.Empty);
            }
            catch(Exception ex)
            {
                if(overall_log != null && overall_log.BaseStream != null && overall_log.BaseStream.CanWrite)
                {
                    overall_log.WriteLine(ex.ToString());
                }
                if(log != null && log.BaseStream != null && log.BaseStream.CanWrite)
                {
                    log.WriteLine(ex.ToString());
                }
                OnThrowException(new ExceptionEventArgs(ex));
            }
            finally
            {
                if(overall_log != null)
                {
                    overall_log.Close();
                }
                if(summary != null)
                {
                    summary.Close();
                }
                if(log != null)
                {
                    log.Close();
                }
                if(protein_fasta_database != null)
                {
                    protein_fasta_database.Close();
                }

                TandemMassSpectra.ReportTaskWithoutProgress -= HandleReportTaskWithoutProgress;
                TandemMassSpectra.ReportTaskWithProgress -= HandleReportTaskWithProgress;
                TandemMassSpectra.UpdateProgress -= HandleUpdateProgress;
            }
        }

        private static SortedList<string, HashSet<string>> DetermineSemiAggregateParentFolders(ICollection<string> dataFilepaths)
        {
            SortedList<string, HashSet<string>> parents = new SortedList<string, HashSet<string>>();

            // find all the parent folders and the data files they contain
            // does this work if the data files are in the root directory (i.e. C:\)?
            foreach(string data_filepath in dataFilepaths)
            {
                string directory = data_filepath;
                while(true)
                {
                    directory = Path.GetDirectoryName(directory);
                    if(directory != null)
                    {
                        HashSet<string> parent_data_filepaths;
                        if(!parents.TryGetValue(directory, out parent_data_filepaths))
                        {
                            parent_data_filepaths = new HashSet<string>();
                            parent_data_filepaths.Add(data_filepath);
                            parents.Add(directory, parent_data_filepaths);
                        }
                        else
                        {
                            parent_data_filepaths.Add(data_filepath);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            // remove parent folders with only one data file or every data file, as they are processed separately and not eligible for semi-aggregate analysis
            int i = 0;
            while(i < parents.Count)
            {
                HashSet<string> parent_data_filepaths = parents.Values[i];
                if(parent_data_filepaths.Count == 1 || parent_data_filepaths.Count == dataFilepaths.Count)
                {
                    parents.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            // remove higher-level parent folders where a lower-level parent folder contains the same data files
            int j = 0;
            while(j < parents.Count - 1)
            {
                HashSet<string> current_data_filepaths = parents.Values[j];

                int k = j + 1;
                while(k < parents.Count)
                {
                    HashSet<string> next_data_filepaths = parents.Values[k];
                    if(next_data_filepaths.SetEquals(current_data_filepaths))
                    {
                        parents.RemoveAt(j);
                        break;
                    }
                    else
                    {
                        k++;
                    }
                }

                j++;
            }

            return parents;
        }
    }
}
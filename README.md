# Morpheus
mass spectrometry–based proteomics database search algorithm

## Requirements
* Windows: [Microsoft .NET Framework](http://www.microsoft.com/net) 4.0 Client Profile or higher (4.5 or higher recommended)
  * [Thermo MSFileReader](https://thermo.flexnetoperations.com/control/thmo/search?query=MSFileReader) for Thermo version
* Linux / Mac OS X: [Mono](http://www.mono-project.com/) 2.8 or later

## Notes
* In revision 83 and later, sites inducing and preventing cleavage must be separated by commas in proteases.tsv, since multi-amino acid sites are now supported.
* In revision 108 and later, proteome databases may be UniProt XML files in addition to FASTA files to support [G-PTM searching](http://pubs.acs.org/doi/abs/10.1021/acs.jproteome.5b00599).
  * When an XML file is loaded, its modifications will be read (masses are read from [ptmlist.txt](http://www.uniprot.org/docs/ptmlist)), added to your variable modifications list, and checked by default.
* In revision 167 and above, an extra column was added to modifications.tsv to allow for neutral loss modification searching.
* In revision 255 and above, mzIdentML support necessitated the need to link every modification to UniMod. Therefore, two extra columns, 'UniMod Accession Number' and 'UniMod Name', were were added to modifications.tsv.

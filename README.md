## About

The goal of this project is to do setup a pipeline for the NPPES roaster data (HIPAA Administrative Simplification: National Plan and Provider Enumeration System Data Dissemination) so that it is ready for data science projects. For context NPPES roaster  data is the provider demographics data for US and other participating countries in the initiative to provide an public provier directory. A provider is anyone who is a licensed practitioner to provider medical care to a patient. 

## Data Source
The data that we are going to explore here is sourced from [here](https://www.cms.gov/Regulations-and-Guidance/Administrative-Simplification/NationalProvIdentStand/DataDissemination)
## License 
The data is licensed under the terms and conditions as mentioned in [this<sup>1</sp>](https://www.cms.gov/Regulations-and-Guidance/Administrative-Simplification/NationalProvIdentStand/Downloads/DataDisseminationNPI.pdf), [this<sup>2</sup>](https://www.cms.gov/Regulations-and-Guidance/Administrative-Simplification/NationalProvIdentStand/DataDissemination) document. Any work carried out here adheres to the licensing terms as stated by federal law (45 CFR Part 162). 

## Sections
We will disucss about the different phases of the ML pipeline and how it is achieved with this dataset in detail in the upcoming sections. 
1. Data Ingestion


## Data Ingestion
### Frequency of Data
This data is updated every week and a weekly incremental update file is published on the website. The structure of the data is not changed. The complete file is also downloadable from the same webpage. Please refer the Data Source section to get the link of the download page.

### Type of data
This data set is structured data with header rows and metadata about the headers. Refer to the data source section for the link of the download page for metadata and data dictionary.

### Storage
For simplicity to use the work across different cloud environments and on-premise we will be using MongoDB and .NET/Python as the technology of choice to do the work. The steps involved in acquiring the data are 
1. 

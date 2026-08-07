namespace AIAnalysis.Application.DTOs;

public record DiseaseDetailsDto(
    string Name,
    bool IsContagious,
    decimal LethalityIndex,
    ContaminationInfo? ContaminationDetails
);

public record ContaminationInfo(
    List<string> Sources,
    List<string> TransmissionMethods
);
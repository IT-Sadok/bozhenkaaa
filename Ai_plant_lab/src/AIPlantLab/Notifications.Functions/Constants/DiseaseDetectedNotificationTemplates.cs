namespace Notifications.Functions.Constants;

public static class DiseaseDetectedNotificationTemplates
{
    public const string SubjectFormat = "Disease detected: {0}";

    public const string BodyFormat =
        "Experiment ID: {0}\nContagious: {1}\nLethality index: {2}\nRecommendations: {3}";
}

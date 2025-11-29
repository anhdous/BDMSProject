namespace ApplicationCore.Models;

public class MedicalHistoryListModel
  {
    public int MID { get; set; }
    public string ConditionName { get; set; }
    public DateTime DiagnosisDate { get; set; }
    public DateTime RecoveryDate { get; set; }
    public string Status { get; set; }
    public int DonorID { get; set; }
  }

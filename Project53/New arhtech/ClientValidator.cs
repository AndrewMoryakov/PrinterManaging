namespace Project53.New_arhtech
{
	public class ClientValidator
	{
		public static bool CanContinuePrinting(Client client, JobMeta job) 
			=> client.Balance >= job.Copies * job.CountOfPages;
	}
}
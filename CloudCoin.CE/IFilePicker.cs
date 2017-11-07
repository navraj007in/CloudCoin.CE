using System;
using System.Threading.Tasks;
namespace CloudCoin.CE
{
	public interface IFilePicker
	{
        string GetHomeFolder();
        Task<FileData> PickFile(string importFilePath);
	}
}

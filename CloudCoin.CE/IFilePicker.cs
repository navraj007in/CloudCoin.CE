using System;
using System.Threading.Tasks;
namespace CloudCoin.CE
{
	public interface IFilePicker
	{
        Task<FileData> PickFile(string importFilePath);
	}
}

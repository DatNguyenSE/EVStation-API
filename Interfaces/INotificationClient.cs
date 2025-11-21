using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces
{
    // Interface này định nghĩa các hàm mà Client (Angular) sẽ "lắng nghe"
    public interface INotificationClient
    {
        Task ReservationCancelled(string message);
        Task NewTaskAssigned(string message);
        
        Task NewReportReceived(Report notificationReport); // Gửi cho Admin
        Task FixCompleted(string message);      // Gửi cho Admin
        Task ReportClosed(string message);      // Gửi cho Admin
        Task TaskCompleted(string message);     // Gửi cho Technician
    }
}
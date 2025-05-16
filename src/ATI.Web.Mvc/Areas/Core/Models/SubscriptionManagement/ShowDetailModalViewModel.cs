using Abp.AutoMapper;
using ATI.MultiTenancy.Payments.Dto;

namespace ATI.Web.Areas.Core.Models.SubscriptionManagement;

[AutoMapFrom(typeof(SubscriptionPaymentProductDto))]
public class ShowDetailModalViewModel : SubscriptionPaymentProductDto
{
}
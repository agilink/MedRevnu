using Abp.Events.Bus.Handlers;
using ATI.MultiTenancy.Subscription;

namespace ATI.MultiTenancy.Payments
{
    public interface ISupportsRecurringPayments : 
        IEventHandler<RecurringPaymentsDisabledEventData>, 
        IEventHandler<RecurringPaymentsEnabledEventData>,
        IEventHandler<SubscriptionUpdatedEventData>,
        IEventHandler<SubscriptionCancelledEventData>
    {

    }
}

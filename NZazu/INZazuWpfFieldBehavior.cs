namespace NZazu
{
    public interface INZazuWpfFieldBehavior
    {
        // we inject the view because sometimes a behavior might access other parts from the view
        // e.g. a MailTo-Behavior on CTRL+ENTER required the value of the field "subject".
        void AttachTo(INZazuWpfField field, INZazuWpfView view);
        void Detach();
    }
}
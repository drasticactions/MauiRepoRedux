using System.ComponentModel;

namespace BindingViewTest;

[DefaultProperty( "Content" )]
[ContentProperty( "Content" )]
public class MyBorder : TemplatedView
{
    private ContentPresenter m_contentPresenter;

    public MyBorder()
    {
        m_contentPresenter = new ContentPresenter();
        this.ControlTemplate = new ControlTemplate( () => m_contentPresenter );
    }

    public static readonly BindableProperty ContentProperty = BindableProperty.Create( nameof( Content ), typeof( View ), typeof( MyBorder ) );

    public View Content
    {
        get => (View)GetValue( ContentProperty );
        set => SetValue( ContentProperty, value );
    }
}
<%@ Page Title="" Language="C#" MasterPageFile="~/Head.Master"  ClassName="Test"  %>

<asp:Content ContentPlaceHolderID="Zone1" runat="server"  >
  
    <form name="input"  method="post" >
        Code: <input type="password" name="Code" />
        <input type="Submit"  value="Reset" />
    </form>

   

  <asp:ExplicitLog  />   
</asp:Content>


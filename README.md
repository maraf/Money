# Money
Money is an outcome logging application written as UAP (UWP) to target both desktop and mobile Windows devices. 
<br />
<a href='//www.microsoft.com/store/apps/9n50xhgw891s?ocid=badge'>Get it from Microsoft Store</a>.

![Preview in large window size](assets/Preview-large.png)

Also, we have an implementation using [Blazor](https://github.com/aspnet/Blazor).

![Preview in web](assets/Preview-blazor.png)

## Blog posts
 - [Blazor and the booting screen](https://www.neptuo.com/blog/2018/04/blazor-boot-screen/).
 - [Blazor and complex component parameters](https://www.neptuo.com/blog/2018/06/blazor-component-parameters/).
 - [Blazor and page parameters](https://www.neptuo.com/blog/2018/11/blazor-page-parameters/).
 - [Blazor and network](https://www.neptuo.com/blog/2019/12/blazor-network-status/).
 - [Blazor and the magic of App.razor](https://www.neptuo.com/blog/2020/03/blazor-app-razor).

## Running the Blazor version locally

The Blazor version is a typical SPA web app with backend api. So you need to run two projects `Money.Api` & `Money.Blazor.Host`. We typically do it from cmd/pwsh:

### Api
```
cd .\src\Money.Api | dotnet watch
```

### Blazor
```
cd .\src\Money.Blazor.Host | dotnet watch
```

## Deploy pipelines
- [Blazor Production](https://github.com/maraf/com.neptuo.money.app/actions/workflows/deploy.yml)
- [Blazor Beta](https://github.com/maraf/com.neptuo.money.app.beta/actions/workflows/deploy.yml)

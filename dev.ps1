wt new-tab -d "$pwd\src\Money.Api" --title "Api" powershell.exe -noexit -command "dotnet run";
wt new-tab -d "$pwd\src\Money.Blazor.Host" --title "Blazor run" powershell.exe -noexit -command "dotnet run --no-build";
wt new-tab -d "$pwd\src\Money.Blazor.Host" --title "Blazor watch" powershell.exe -noexit -command "dotnet watch build";
exit;
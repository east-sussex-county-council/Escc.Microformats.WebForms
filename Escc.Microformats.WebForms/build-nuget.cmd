@echo off
set nuspec="%1"
set nuspec=%nuspec:\=\\%
nuget pack "%nuspec%Escc.Microformats.WebForms.nuspec"
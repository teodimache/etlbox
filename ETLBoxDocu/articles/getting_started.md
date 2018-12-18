# Introduction

This introductional page will gives you an introduction to the basic concepts of ETLBox.

## ETLBox components

ETLBox is split into two main components: Control Flow Tasks and Data Flow Tasks. Some taks in the Control Flow part are for logging purposes only, 
they will be explained in detail in its own artice.

### Namespaces

Everything in ETLBox except the Data Flow tasks are within the namespace `ALE.ETLBox`.
All Data Flow components (sometimes referred as components) can be found in the namespace `ALE.ETLBox.DataFlow`.

### Clean coding

At first glance, it can look like that some of the taks are not really adding big value to your project. But in accordance to clean coding, 
having written everything in a format which is easy-to-read and easy-to-understand, you are able to produce code that is easiert to maintain. Having your code "clean" 
allows you to focus on the important parts of your logic and not be distracted by code lines that your brain need to "unravel" every time you go over them.
If you are interested in clean coding, you should read the "bible" on that topic, writte by Robert C. Martin: [Clean Code](amazonlinkhere)

# Getting started

## Overview Control Flow Tasks

You will find an introduction into the Control Flow Tasks in the artice [Overview Control Flow](overview_controlflow.md).
This will give you all the basics you need to understand how the Control Flow tasks are designed.
If you want to dig deeper, please see the API reference for detailled information about the tasks. 
If you are in need of some examples of how to use Control Flow tasks, see the [Example Control Flow](example_controlflow.md)

## Overview Logging 

All Control Flow and Data Flow Tasks come with the ability to produce log. 
There are also some special task that enables you to create or query the log tables easily. 
To get an introduction into logging, please have a look at the [Overview Logging](overview_logging.md)
All logging capabilites are based on nlog. You can visit the [NLog homepage](nlog-project.org) if you are interested in more details how to set up and configure NLog.

## Overview Data Flow Tasks

All components in the Data Flow allow you to create your ETL (Extract, Transform, Load) pipeline - 
where data is extracted from the source(s), asynchrounously transformed and then loaded into your destinations.
Plese read the [Overview Data Flow](overview_dataflow.md) to get started. There is also an [Example Data Flow](https://github.com/roadrunnerlenny/etlbox/wiki/Example-Data-Flow).
To understand the dataflow components, you can also visit the API reference and look at the description and details of each dataflow component.

# API Reference

If you are in doubt how to use a certain task, you can have a look at the API reference. All property and method names should be self explanatory and 
already give you a quite good understanding of the code.

## Creating your own task

It is possible to create your own tasks. If you are in need for further details, please give me some feedback via github (open an issuee) and 
I will happily give you detailled instructions how to do so. 










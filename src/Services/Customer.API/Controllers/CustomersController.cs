using Customer.API.Repositories.Interfaces;
using Customer.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Customer.API.Controllers;

public static class CustomersController
{
    public static void MapCustomersAPI ( this WebApplication app )
    {
        app.MapGet("/", ( ) => "Welcome to Customer API!");
        app.MapGet("/api/customers", async ( ICustomerService customerService ) => await customerService.GetCustomersAsync());
        app.MapGet("/api/customers/{username}", async ( string username, ICustomerService customerService ) =>
        {
            var customer = await customerService.GetCustomerByUsernameAsync(username);
            return customer != null ? Results.Ok(customer) : Results.NotFound();
        });


        //app.MapPost("/api/customers", ( ) => async ( Customer.API.Entities.Customer customer, ICustomerRepository customerRepository ) =>
        //{
        //    customerRepository.CreateAsync(customer);
        //    customerRepository.SaveChangesAsync();
        //    return Results.Ok(customer);
        //});

        app.MapPut("/api/customers/{id}", ( ) =>
        {

        });
        //app.MapDelete("/api/customers/{id}", async ( int id, ICustomerRepository customerRepository ) =>
        //{
        //    var customer = await customerRepository.FindByCondition(x => x.Id.Equals(id)).SingleOrDefaultAsync();
        //    if (customer == null) return Results.NotFound();

        //    await customerRepository.DeleteAsync(customer);
        //    await customerRepository.SaveChangesAsync();

        //    return Results.NoContent();
        //});
    }
}

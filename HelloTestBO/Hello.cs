using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using HelloTestDTO;

namespace HelloTestBO
{
    public class Hello
    {
        public string GetHelloWorld()
        {
            return "Hello World EasyArchitect Web API Framework for .NET Standard 2.0.";
        }

        public IEnumerable<Employee> GetEmployees()
        {
            return new Employee[] {
                new Employee() {
                    EmpId = "F123456789",
                    EmpName = "柯文哲",
                    Organization = "開發部"
                },
                new Employee() {
                    EmpId = "A123456789",
                    EmpName = "test",
                    Organization = "開發部"
                },
                new Employee() {
                    EmpId = "I123456789",
                    EmpName = "Gelis",
                    Organization = "開發部"
                }
            }.ToList();
        }

        public int GetApplePrice(Apple apple)
        {
            return apple.Price;
        }
    }
}

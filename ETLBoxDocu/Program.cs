using System;

namespace ALE.ETLBoxDemo {
    class Program {
        static void Main(string[] args) {

            Console.WriteLine("Starting ControlFlow example");
            ControlFlowTasks cft = new ControlFlowTasks();
            cft.Start();

            Console.WriteLine("Start Logging example");
            Logging log = new Logging();
            log.Start();

            Console.WriteLine("Starting DataFlow example");
            DataFlowTasks dft = new DataFlowTasks();
            dft.Preparation();
            dft.Start();
        }
    }
}

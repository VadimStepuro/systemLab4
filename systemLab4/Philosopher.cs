using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace systemLab4
{
    public class Philosopher
    {
        Semaphore semaphore = new Semaphore(1,1);
        Semaphore[] semaphores= new Semaphore[5];
        Rectangle[] philosopherRectangles;
        Rectangle[] forksRectangles;
        int[] philosopherState = new int[5];
        Thread[] philosophers = new Thread[5];
        int minTime;
        int maxTime;
        long totalTime;
        Stopwatch ts = new Stopwatch();
        Dispatcher dispatcher;
        TextBox textBlock;
        Random random = new Random();

        public Philosopher(int minTime, int maxTime, long totalTime, Dispatcher dispatcher, TextBox textBlock, Rectangle[] rectangles, Rectangle[] forksRectangles)
        {
            this.dispatcher = dispatcher;
            this.minTime = minTime;
            this.maxTime = maxTime;
            this.totalTime = totalTime;
            this.textBlock = textBlock;
            this.forksRectangles = forksRectangles;
            for(int i = 0; i < 5; i++) {
                semaphores[i] = new Semaphore(0, 1);
            }
            this.philosopherRectangles = rectangles;
        }

        public void Start()
        {
            for(int i = 0; i < 5; i++) {
                philosophers[i] = new Thread((i) => { Live((int)i); });
                philosophers[i].Start(i);
            }
        }

        public void Live(int philosopher)
        {
            ts.Start();
            Stopwatch thinkingTime = new Stopwatch();
            Stopwatch hungryTime = new Stopwatch();
            Stopwatch eatingTime = new Stopwatch();

            while (ts.ElapsedMilliseconds < totalTime) {
                thinkingTime.Start();
                Think(philosopher);
                thinkingTime.Stop();
                hungryTime.Start();
                TakeForks(philosopher);
                hungryTime.Stop();
                eatingTime.Start();
                Eat(philosopher);
                eatingTime.Stop();
                PutForks(philosopher);
            }
            dispatcher.Invoke(() => { textBlock.Text += "Philosopher " + philosopher + " was thinking for" + thinkingTime.ElapsedMilliseconds + '\n'; });
            dispatcher.Invoke(() => { textBlock.Text += "Philosopher " + philosopher + " was hungry " + hungryTime.ElapsedMilliseconds + '\n'; });
            dispatcher.Invoke(() => { textBlock.Text += "Philosopher " + philosopher + " was eating for" + thinkingTime.ElapsedMilliseconds + '\n'; });
            dispatcher.Invoke(() => { philosopherRectangles[philosopher].Fill = new SolidColorBrush(Colors.White); });
        }

        private void Think(int philosopher)
        {
            dispatcher.Invoke(() => { textBlock.Text += "Philosopher " + philosopher + " is thinking" + '\n'; });
            dispatcher.Invoke(() => { philosopherRectangles[philosopher].Fill = new SolidColorBrush(Colors.Blue); });
            Thread.Sleep(random.Next(minTime, maxTime));
        }

        private void TakeForks(int philosopher)
        {
            semaphore.WaitOne();
            philosopherState[philosopher] = 1;
            dispatcher.Invoke(() => { philosopherRectangles[philosopher].Fill = new SolidColorBrush(Colors.Red); });
            Test(philosopher);
            semaphore.Release();
            semaphores[philosopher].WaitOne();
        }

        private void PutForks(int philosopher)
        {
            dispatcher.Invoke(() => { textBlock.Text += "Philosopher " + philosopher + " is putting forks" + '\n'; });
            semaphore.WaitOne();
            dispatcher.Invoke(() =>
            {
                forksRectangles[(philosopher + 4) % 5].Fill = new SolidColorBrush(Colors.White);
            });
            dispatcher.Invoke(() =>
            {
                forksRectangles[(philosopher) % 5].Fill = new SolidColorBrush(Colors.White);
            });
            philosopherState[philosopher] = 0;
            Test((philosopher +4) % 5);
            Test((philosopher + 1) % 5);
            semaphore.Release();
        }

        private void Eat(int philosopher)
        {
            dispatcher.Invoke(() => { textBlock.Text += "Philosopher " + philosopher + " is eating" + '\n'; });
            dispatcher.Invoke(() => { philosopherRectangles[philosopher].Fill = new SolidColorBrush(Colors.Green); });
            Thread.Sleep(random.Next(minTime, maxTime));
        }

        private void Test(int philosopher)
        {
            if (philosopherState[philosopher] == 1 && philosopherState[(philosopher + 4) % 5] != 2 && philosopherState[(philosopher + 1) % 5] != 2) {
                philosopherState[philosopher] = 2;
                dispatcher.Invoke(() => { textBlock.Text += "Philosopher " + philosopher + " is taking forks" + '\n'; });
                dispatcher.Invoke(() =>
                {
                    forksRectangles[(philosopher + 4) % 5].Fill = new SolidColorBrush(Colors.Green);
                });
                dispatcher.Invoke(() =>
                {
                    forksRectangles[(philosopher) % 5].Fill = new SolidColorBrush(Colors.Green);
                });
                semaphores[philosopher].Release();
            }
        }

    }
}

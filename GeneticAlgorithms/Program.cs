using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticAlgorithms
{
    class Program
    {
        const double CROSSOVER_RATE = 0.7;
        const double MUTATION_RATE = 0.001;
        const int POP_SIZE = 100;		//must be an even number
        const int CHROMO_LENGTH = 300;
        const int GENE_LENGTH = 4;
        const int MAX_ALLOWABLE_GENERATIONS = 400;

        static Random r;

        public class chromo
        {
            public string bits;
            public float fitness;

            public chromo()
            {
                bits = "";
                fitness = 0.0f;
            }
            public chromo(string bts, float ftns)
            {
                bits = bts;
                fitness = ftns;
            }
        }

        static void Main(string[] args)
        {
            //seed the random number generator
            r = new Random();

            //just loop endlessly until user gets bored :0)
            while (true)
            {
                //storage for our population of chromosomes.
                chromo[] Population = new chromo[POP_SIZE];

                //get a target number from the user. (no error checking)
                float Target;
                System.Console.Out.WriteLine("\nInput a target number: ");
                Target = float.Parse(System.Console.In.ReadLine());

                //first create a random population, all with zero fitness.
                for (int i = 0; i < POP_SIZE; i++)
                {
                    Population[i] = new chromo(GetRandomBits(CHROMO_LENGTH), 0.0f);
                }

                int GenerationsRequiredToFindASolution = 0;

                //we will set this flag if a solution has been found
                bool bFound = false;

                //enter the main GA loop
                while (!bFound)
                {
                    //this is used during roulette wheel sampling
                    float TotalFitness = 0.0f;

                    // test and update the fitness of every chromosome in the 
                    // population
                    for (int i = 0; i < POP_SIZE; i++)
                    {
                        Population[i].fitness = AssignFitness(Population[i].bits, Target);
                        TotalFitness += Population[i].fitness;
                    }

                    // check to see if we have found any solutions (fitness will be 999)
                    for (int i = 0; i < POP_SIZE; i++)
                    {
                        if (Population[i].fitness == 999.0f)
                        {
                            System.Console.Out.WriteLine("\nSolution found in " + GenerationsRequiredToFindASolution + " generations!");
                            PrintChromo(Population[i].bits);
                            bFound = true;
                            break;
                        }
                    }

                    // create a new population by selecting two parents at a time and creating offspring
                    // by applying crossover and mutation. Do this until the desired number of offspring
                    // have been created. 

                    //define some temporary storage for the new population we are about to create
                    chromo[] temp = new chromo[POP_SIZE];

                    int cPop = 0;

                    //loop until we have created POP_SIZE new chromosomes
                    while (cPop < POP_SIZE)
                    {
                        // we are going to create the new population by grabbing members of the old population
                        // two at a time via roulette wheel selection.
                        string offspring1 = Roulette(TotalFitness, Population);
                        string offspring2 = Roulette(TotalFitness, Population);

                        //add crossover dependent on the crossover rate
                        Crossover(ref offspring1, ref offspring2);

                        //now mutate dependent on the mutation rate
                        Mutate(ref offspring1);
                        Mutate(ref offspring2);

                        //add these offspring to the new population. (assigning zero as their
                        //fitness scores)
                        temp[cPop++] = new chromo(offspring1, 0.0f);
                        temp[cPop++] = new chromo(offspring2, 0.0f);

                    }//end loop

                    //copy temp population into main population array
                    for (int i = 0; i < POP_SIZE; i++)
                    {
                        Population[i] = temp[i];
                    }

                    ++GenerationsRequiredToFindASolution;

                    // exit app if no solution found within the maximum allowable number
                    // of generations
                    if (GenerationsRequiredToFindASolution > MAX_ALLOWABLE_GENERATIONS)
                    {
                        System.Console.Out.WriteLine("No solutions found this run!");
                        bFound = true;
                    }
                }
            }//end while
        }
        //---------------------------------GetRandomBits-----------------------------------------
        //
        //	This function returns a string of random 1s and 0s of the desired length.
        //
        //-----------------------------------------------------------------------------------------
        private static string GetRandomBits(int length)
        {
            string bits = "";

            for (int i = 0; i < length; i++)
            {
                if (r.NextDouble() > 0.5f)
                    bits += "1";
                else
                    bits += "0";
            }
            return bits;
        }

        //---------------------------------BinToDec-----------------------------------------
        //
        //	converts a binary string into a decimal integer
        //
        //-----------------------------------------------------------------------------------
        private static int BinToDec(string bits)
        {
            int val = 0;
            int value_to_add = 1;

            for (int i = bits.Length; i > 0; i--)
            {
                if (bits[i - 1] == '1')
                    val += value_to_add;
                value_to_add *= 2;
            }//next bit

            return val;
        }
        //---------------------------------ParseBits------------------------------------------
        //
        // Given a chromosome this function will step through the genes one at a time and insert 
        // the decimal values of each gene (which follow the operator -> number -> operator rule)
        // into a buffer. Returns the number of elements in the buffer.
        //------------------------------------------------------------------------------------
        private static int ParseBits(string bits, int[] buffer)
        {
            //counter for buffer position
            int cBuff = 0;
            // step through bits a gene at a time until end and store decimal values
            // of valid operators and numbers. Don't forget we are looking for operator - 
            // number - operator - number and so on... We ignore the unused genes 1111
            // and 1110

            //flag to determine if we are looking for an operator or a number
            bool bOperator = false;

            //storage for decimal value of currently tested gene
            int this_gene = 0;

            for (int i = 0; i < CHROMO_LENGTH; i += GENE_LENGTH)
            {
                //convert the current gene to decimal
                this_gene = BinToDec(bits.Substring(i, GENE_LENGTH));

                //find a gene which represents an operator
                if (bOperator)
                {
                    if ((this_gene < 10) || (this_gene > 13))
                        continue;
                    else
                    {
                        bOperator = false;
                        buffer[cBuff++] = this_gene;
                        continue;
                    }
                }
                //find a gene which represents a number
                else
                {
                    if (this_gene > 9)
                        continue;
                    else
                    {
                        bOperator = true;
                        buffer[cBuff++] = this_gene;
                        continue;
                    }
                }
            }//next gene

            //	now we have to run through buffer to see if a possible divide by zero
            //	is included and delete it. (ie a '/' followed by a '0'). We take an easy
            //	way out here and just change the '/' to a '+'. This will not effect the 
            //	evolution of the solution
            for (int i = 0; i < cBuff; i++)
            {
                if ((buffer[i] == 13) && (buffer[i + 1] == 0))
                    buffer[i] = 10;
            }
            return cBuff;
        }
        //---------------------------------AssignFitness--------------------------------------
        //
        //	given a string of bits and a target value this function will calculate its  
        //  representation and return a fitness score accordingly
        //------------------------------------------------------------------------------------
        private static float AssignFitness(string bits, float target_value)
        {

            //holds decimal values of gene sequence
            int[] buffer = new int[(int)(CHROMO_LENGTH / GENE_LENGTH)];

            int num_elements = ParseBits(bits, buffer);

            // ok, we have a buffer filled with valid values of: operator - number - operator - number..
            // now we calculate what this represents.
            float result = 0.0f;

            for (int i = 1; i < num_elements; i += 2)
            {
                switch (buffer[i])
                {
                    case 10:
                        result += buffer[i + 1];
                        break;
                    case 11:
                        result -= buffer[i + 1];
                        break;
                    case 12:
                        result *= buffer[i + 1];
                        break;
                    case 13:
                        result /= buffer[i + 1];
                        break;
                }//end switch
            }

            // Now we calculate the fitness. First check to see if a solution has been found
            // and assign an arbitarily high fitness score if this is so.

            if (result == (float)target_value)
                return 999.0f;
            else
            {
                if (target_value > result)
                {
                    return 1 / (float)(target_value - result);
                }
                else
                {
                    return 1 / (float)(result - target_value);
                }
            }

            //	return result;
        }

        //---------------------------------PrintChromo---------------------------------------
        //
        // decodes and prints a chromo to screen
        //-----------------------------------------------------------------------------------
        private static void PrintChromo(string bits)
        {
            //holds decimal values of gene sequence
            int[] buffer = new int[(int)(CHROMO_LENGTH / GENE_LENGTH)];

            //parse the bit string
            int num_elements = ParseBits(bits, buffer);

            for (int i = 0; i < num_elements; i++)
            {
                PrintGeneSymbol(buffer[i]);
            }

            return;
        }

        //--------------------------------------PrintGeneSymbol-----------------------------
        //	
        //	given an integer this function outputs its symbol to the screen 
        //----------------------------------------------------------------------------------
        private static void PrintGeneSymbol(int val)
        {
            if (val < 10)
                System.Console.Write(val + " ");
            else
            {
                switch (val)
                {
                    case 10:
                        System.Console.Write("+");
                        break;
                    case 11:
                        System.Console.Write("-");
                        break;
                    case 12:
                        System.Console.Write("*");
                        break;
                    case 13:
                        System.Console.Write("/");
                        break;
                }//end switch

                System.Console.Write(" ");
            }
            return;
        }

        //------------------------------------Mutate---------------------------------------
        //
        //	Mutates a chromosome's bits dependent on the MUTATION_RATE
        //-------------------------------------------------------------------------------------
        private static void Mutate(ref string bits)
        {
            for (int i = 0; i < bits.Length; i++)
            {
                if (r.NextDouble() < MUTATION_RATE)
                {
                    if (bits[i] == '1')
                    {
                        bits = bits.Substring(0, i) + "0" + (i + 1 < bits.Length ? bits.Substring(i + 1) : "");
                    }
                    else
                    {
                        bits = bits.Substring(0, i) + "1" + (i + 1 < bits.Length ? bits.Substring(i + 1) : "");
                    }
                }
            }
            return;
        }

        //---------------------------------- Crossover ---------------------------------------
        //
        //  Dependent on the CROSSOVER_RATE this function selects a random point along the 
        //  lenghth of the chromosomes and swaps all the  bits after that point.
        //------------------------------------------------------------------------------------
        private static void Crossover(ref string offspring1, ref string offspring2)
        {
            //dependent on the crossover rate
            if (r.NextDouble() < CROSSOVER_RATE)
            {
                //create a random crossover point
                int crossover = (int)(r.NextDouble() * CHROMO_LENGTH);

                string t1 = offspring1.Substring(0, crossover) + offspring2.Substring(crossover, CHROMO_LENGTH - crossover);
                string t2 = offspring2.Substring(0, crossover) + offspring1.Substring(crossover, CHROMO_LENGTH - crossover);

                offspring1 = t1; offspring2 = t2;
            }
        }


        //--------------------------------Roulette-------------------------------------------
        //
        //	selects a chromosome from the population via roulette wheel selection
        //------------------------------------------------------------------------------------
        private static string Roulette(float total_fitness, chromo[] Population)
        {
            //generate a random number between 0 & total fitness count
            float Slice = (float)(r.NextDouble() * total_fitness);

            //go through the chromosones adding up the fitness so far
            float FitnessSoFar = 0.0f;

            for (int i = 0; i < POP_SIZE; i++)
            {
                FitnessSoFar += Population[i].fitness;
                //if the fitness so far > random number return the chromo at this point
                if (FitnessSoFar >= Slice)
                    return Population[i].bits;
            }
            return "";
        }
    }
}

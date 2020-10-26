using System.Text;

namespace Nocturne
{
    /// <summary>
    /// string builder that doesn't create garbage when appending numbers
    /// </summary>
    public class NStringBuilder
    {
        private static readonly char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private readonly char[] numberBuffer = new char[25];

        /// <summary>
        /// The actual string builder used 
        /// </summary>
        public StringBuilder StringBuilder { get; }

        /// <summary>
        /// The string value for this string builder
        /// </summary>
        public string Value => StringBuilder.ToString();

        public NStringBuilder( )
        {
            StringBuilder = new StringBuilder();
        }

        /// <summary>
        /// appends integer without creating garbage
        /// </summary>
        public NStringBuilder AppendNumber( int number )
        {
            if ( number < 0 )
            {
                StringBuilder.Append( "-" );
                number = -number;
            }

            int index = 0;

            do
            {
                int digit = number % 10;
                if ( ( index + 1 ) % 4 == 0 )
                {
                    numberBuffer[index] = (char)( ',' );
                    ++index;
                }
                numberBuffer[index] = digits[digit];
                number /= 10;
                ++index;
            } while ( number > 0 );

            for ( --index; index >= 0; --index )
            {
                StringBuilder.Append( numberBuffer[index] );
            }

            return this;
        }

        /// <summary>
        /// appends long without creating garbage
        /// </summary>
        public NStringBuilder AppendNumber( long number )
        {
            if ( number < 0 )
            {
                StringBuilder.Append( "-" );
                number = -number;
            }

            int index = 0;

            do
            {
                long digit = number % 10;
                if ( ( index + 1 ) % 4 == 0 )
                {
                    numberBuffer[index] = (char)( ',' );
                    ++index;
                }
                numberBuffer[index] = digits[digit];
                number /= 10;
                ++index;
            } while ( number > 0 );

            for ( --index; index >= 0; --index )
            {
                StringBuilder.Append( numberBuffer[index] );
            }

            return this;
        }

        /// <summary> 
        /// Appends a 2 decimal floating point number without creating garbage. 
        /// </summary> 
        public NStringBuilder AppendNumber( float number )
        {
            number *= 100f;
            if ( number < 0 )
            {
                StringBuilder.Append( "-" );
                number = -number;
            }
            float original = number;

            int index = 0;
            do
            {
                if ( index == 2 )
                {
                    numberBuffer[index] = (char)( '.' );
                    ++index;
                }

                int digit = (int)number % 10;
                numberBuffer[index] = (char)( '0' + digit );
                number /= 10;
                ++index;
            } while ( number > 0.99f );

            if ( original < 100 )
            {
                if ( original < 10 )
                {
                    numberBuffer[index] = (char)( '0' );
                    ++index;
                }

                numberBuffer[index] = (char)( '.' );
                ++index;
                numberBuffer[index] = (char)( '0' );
                ++index;
            }

            for ( --index; index >= 0; --index )
            {
                StringBuilder.Append( numberBuffer[index] );
            }
            return this;
        }

        public NStringBuilder Append( string text )
        {
            StringBuilder.Append( text );
            return this;
        }

        public NStringBuilder Append( string text1, string text2 )
        {
            StringBuilder.Append( text1 ).Append( text2 );
            return this;
        }

        public NStringBuilder Append( string text1, string text2, string text3 )
        {
            StringBuilder.Append( text1 ).Append( text2 ).Append( text3 );
            return this;
        }

        public NStringBuilder Append( string text1, string text2, string text3, string text4 )
        {
            StringBuilder.Append( text1 ).Append( text2 ).Append( text3 ).Append( text4 );
            return this;
        }

        public NStringBuilder Append( string text1, string text2, string text3, string text4, string text5 )
        {
            StringBuilder.Append( text1 ).Append( text2 ).Append( text3 ).Append( text4 ).Append( text5 );
            return this;
        }

        public void Clear()
        {
            StringBuilder.Remove( 0, StringBuilder.Length );
        }

        public override string ToString() => Value;
    }
}

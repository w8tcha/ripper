// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAPICodePack.Sensors
{
    /// <summary>
    /// Defines a strongly typed list of sensors.
    /// </summary>
    /// <typeparam name="S">The type of sensor in the list.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "S"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
    public class SensorList<S> : IList<S>
        where S : Sensor
    {
        private List<S> sensorList = new List<S>( );

        #region IList<S> Members

        /// <summary>
        /// Returns a sensor at a particular index.
        /// </summary>
        /// <param name="item">The sensor item.</param>
        /// <returns>The index of the sensor.</returns>
        public int IndexOf( S item )
        {
            return sensorList.IndexOf( item );
        }

        /// <summary>
        /// Inserts a sensor at a specific location in the list.
        /// </summary>
        /// <param name="index">The index to insert the sensor.</param>
        /// <param name="item">The sensor to insert.</param>
        public void Insert( int index, S item )
        {
            sensorList.Insert( index, item );
        }

        /// <summary>
        /// Removes a sensor at a specific location in the list.
        /// </summary>
        /// <param name="index">The index of the sensor to remove.</param>
        public void RemoveAt( int index )
        {
            sensorList.RemoveAt( index );
        }

        /// <summary>
        /// Gets or sets a sensor at a specified location in the list.
        /// </summary>
        /// <param name="index">The index of the sensor in the list.</param>
        /// <returns>The sensor.</returns>
        public S this[ int index ]
        {
            get
            {
                return sensorList[ index ];
            }
            set
            {
                sensorList[ index ] = value;
            }
        }

        #endregion

        #region ICollection<S> Members

        /// <summary>
        /// Adds a sensor to the end of the list.
        /// </summary>
        /// <param name="item">The sensor item.</param>
        public void Add( S item )
        {
            sensorList.Add( item );
        }

        /// <summary>
        /// Clears the list of sensors.
        /// </summary>
        public void Clear( )
        {
            sensorList.Clear( );
        }

        /// <summary>
        /// Determines if the list contains a specified sensor.
        /// </summary>
        /// <param name="item">The sensor to locate.</param>
        /// <returns><b>true</b> if the list contains the sensor; otherwise <b>false</b>.</returns>
        public bool Contains( S item )
        {
            return sensorList.Contains( item );
        }

        /// <summary>
        /// Copies a sensor to a new list.
        /// </summary>
        /// <param name="array">The destination list.</param>
        /// <param name="arrayIndex">The index of the item to copy.</param>
        public void CopyTo( S[ ] array, int arrayIndex )
        {
            sensorList.CopyTo( array, arrayIndex );
        }

        /// <summary>
        /// Gets the number of items in the list.
        /// </summary>
        public int Count
        {
            get
            { 
                return sensorList.Count;
            }
        }

        /// <summary>
        /// Gets a value that determines if the list is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return ( sensorList as ICollection<S> ).IsReadOnly;
            }
        }

        /// <summary>
        /// Removes a specific sensor from the list.
        /// </summary>
        /// <param name="item">The sensor to remove.</param>
        /// <returns><b>true</b> if the sensor was removed from the list; otherwise <b>false</b>.</returns>
        public bool Remove( S item )
        {
            return sensorList.Remove( item ); ;
        }

        #endregion

        #region IEnumerable<S> Members

        /// <summary>
        /// Returns an enumerator for the list.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator<S> GetEnumerator( )
        {
            return ( sensorList.GetEnumerator() );
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator for the list.
        /// </summary>
        /// <returns>An enumerator.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator( )
        {
            return sensorList.GetEnumerator( );
        }

        #endregion
    }
}

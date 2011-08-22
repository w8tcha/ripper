// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Sensors
{
    /// <summary>
    /// Represents the method that will handle the DataReportChanged event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DataReportChangedEventHandler( Sensor sender, EventArgs e );

    /// <summary>
    /// Represents the method that will handle the StatChanged event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void StateChangedEventHandler( Sensor sender, EventArgs e );

    /// <summary>
    /// Defines a general wrapper for a sensor.
    /// </summary>
    public class Sensor : ISensorEvents
    {
        /// <summary>
        /// Occurs when the DataReport member changes.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly",
            Justification="The event is raised by a static method, so passing in the sender instance is not possible")]
        public event DataReportChangedEventHandler DataReportChanged;

        /// <summary>
        /// Occurs when the State member changes.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly",
            Justification="The event is raised by a static method, so passing in the sender instance is not possible")]
        public event StateChangedEventHandler StateChanged;


        #region Public properties

        /// <summary>
        /// Gets a value that specifies the most recent data reported by the sensor.
        /// </summary>
        public SensorReport DataReport
        {
            get
            {
                return dataReport;
            }
        }
        SensorReport dataReport = null;

        /// <summary>
        /// Gets a value that specifies the GUID for the sensor instance.
        /// </summary>
        public Guid? SensorId
        {
            get
            {
                if( sensorId == null )
                {
                    Guid id;
                    HRESULT hr = nativeISensor.GetID( out id );
                    if( hr == HRESULT.S_OK )
                    {
                        sensorId = id;
                    }
                }
                return sensorId;
            }
        }
        private Guid? sensorId = null;

        /// <summary>
        /// Gets a value that specifies the GUID for the sensor category.
        /// </summary>
        public Guid? CategoryId
        {
            get
            {
                if( categoryId == null )
                {
                    Guid id;
                    HRESULT hr = nativeISensor.GetCategory( out id );
                    if( hr == HRESULT.S_OK )
                    {
                        categoryId = id;
                    }
                }

                return categoryId;
            }
        }
        private Guid? categoryId = null;

        /// <summary>
        /// Gets a value that specifies the GUID for the sensor type.
        /// </summary>
        public Guid? TypeId
        {
            get
            {
                if( typeId == null )
                {
                    Guid id;
                    HRESULT hr = nativeISensor.GetType( out id );
                    if( hr == HRESULT.S_OK )
                        typeId = id;
                }

                return typeId;
            }
        }
        private Guid? typeId = null;

        /// <summary>
        /// Gets a value that specifies the sensor's friendly name.
        /// </summary>
        public string FriendlyName
        {
            get
            {
                if( friendlyName == null )
                {
                    string name = null;
                    HRESULT hr = nativeISensor.GetFriendlyName( out name );
                    if( hr == HRESULT.S_OK )
                        friendlyName = name;
                }
                return friendlyName;
            }
        }
        private string friendlyName = null;

        /// <summary>
        /// Gets a value that specifies the sensor's current state.
        /// </summary>
        public SensorState State
        {
            get
            {
                NativeSensorState state;
                nativeISensor.GetState( out state );
                return (SensorState)state;
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies the report interval.
        /// </summary>
        public uint ReportInterval
        {
            get
            {
                return (uint)GetProperty( SensorPropertyKeys.SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL );
            }

            set
            {
                SetProperties( new DataFieldInfo[ ] { new DataFieldInfo( SensorPropertyKeys.SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL, value ) } );
            }
        }

        /// <summary>
        /// Gets a value that specifies the minimum report interval.
        /// </summary>
        public uint MinimumReportInterval
        {
            get
            {
                return (uint)GetProperty( SensorPropertyKeys.SENSOR_PROPERTY_MIN_REPORT_INTERVAL );
            }
        }

        /// <summary>
        /// Gets a value that specifies the manufacturer of the sensor.
        /// </summary>
        public string Manufacturer
        {
            get
            {
                if( manufacturer == null )
                {
                    manufacturer = (string)GetProperty( SensorPropertyKeys.SENSOR_PROPERTY_MANUFACTURER );
                }
                return manufacturer;
            }
        }
        private string manufacturer = null;

        /// <summary>
        /// Gets a value that specifies the sensor's model.
        /// </summary>
        public string Model
        {
            get
            {
                if( model == null )
                {
                    model = (string)GetProperty( SensorPropertyKeys.SENSOR_PROPERTY_MODEL );
                }
                return model;
            }
        }
        private string model = null;

        /// <summary>
        /// Gets a value that specifies the sensor's serial number.
        /// </summary>
        public string SerialNumber
        {
            get
            {
                if( serialNumber== null )
                {
                    serialNumber = (string)GetProperty( SensorPropertyKeys.SENSOR_PROPERTY_SERIAL_NUMBER );
                }
                return serialNumber;
            }
        }
        private string serialNumber = null;

        /// <summary>
        /// Gets a value that specifies the sensor's description.
        /// </summary>
        public string Description
        {
            get
            {
                if( description == null )
                {
                    description = (string)GetProperty( SensorPropertyKeys.SENSOR_PROPERTY_DESCRIPTION );
                }

                return description;
            }
        }
        private string description = null;

        /// <summary>
        /// Gets a value that specifies the sensor's connection type.
        /// </summary>
        public SensorConnectionType? ConnectionType
        {
            get
            {
                if( connectionType == null )
                {
                    connectionType = (SensorConnectionType)GetProperty( SensorPropertyKeys.SENSOR_PROPERTY_CONNECTION_TYPE );
                }
                return connectionType;
            }
        }
        private SensorConnectionType? connectionType = null;

        /// <summary>
        /// Gets a value that specifies the sensor's device path.
        /// </summary>
        public string DevicePath
        {
            get
            {
                if( devicePath == null )
                {
                    devicePath = (string)GetProperty( SensorPropertyKeys.SENSOR_PROPERTY_DEVICE_ID );
                }

                return devicePath;
            }
        }
        private string devicePath = null;

        /// <summary>
        /// Gets or sets a value that specifies whether the data should be automatically updated.   
        /// If the value is not set, call TryUpdateDataReport or UpdateDataReport to update the DataReport member.
        /// </summary>
        public bool AutoUpdateDataReport
        {
            get
            {
                return IsEventInterestSet( EventInterestTypes.DataUpdated );
            }
            set
            {
                if( value )
                    SetEventInterest( EventInterestTypes.DataUpdated );
                else
                    ClearEventInterest( EventInterestTypes.DataUpdated );
            }
        }

        #endregion

        #region public methods
        /// <summary>
        /// Attempts a synchronous data update from the sensor.
        /// </summary>
        /// <returns><b>true</b> if the request was successful; otherwise <b>false</b>.</returns>
        public bool TryUpdateData( )
        {
            HRESULT hr = InternalUpdateData( );
            return (hr == HRESULT.S_OK);
        }

        /// <summary>
        /// Requests a synchronous data update from the sensor. The method throws an exception if the request fails.
        /// </summary>
        public void UpdateData( )
        {
            HRESULT hr = InternalUpdateData( );
            if( hr != HRESULT.S_OK )
            {
                throw new SensorPlatformException( "Unable to get report from sensor", Marshal.GetExceptionForHR((int)hr) );
            }
        }

        internal HRESULT InternalUpdateData()
        {
            
            ISensorDataReport iReport = null;
            HRESULT hr = nativeISensor.GetData( out iReport );
            if( hr == HRESULT.S_OK )
            {
                try
                {
                    this.dataReport = SensorReport.FromNativeReport( this, iReport );
                    if( DataReportChanged != null )
                    {
                        DataReportChanged.Invoke( this, EventArgs.Empty );
                    }
                }
                finally
                {
                    Marshal.ReleaseComObject( iReport );
                }
            }
            return hr;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])")]
        public override string ToString( )
        {
            return String.Format(
                "Sensor id='{0}' type='{1}' category='{2}' name='{3}'.",
                this.SensorId,
                this.TypeId,
                this.CategoryId,
                this.FriendlyName );
        }

        
        /// <summary>
        /// Retrieves a property value by the property key.
        /// </summary>
        /// <param name="propKey">A property key.</param>
        /// <returns>A property value.</returns>
        public object GetProperty( PropertyKey propKey )
        {
            PropVariant pv;
            HRESULT hr = nativeISensor.GetProperty( ref propKey, out pv );
            if( hr != HRESULT.S_OK )
            {
                Exception e = Marshal.GetExceptionForHR( (int)hr );
                if( hr == HRESULT.E_ELEMENTNOTFOUND )
                {
                    throw new ArgumentOutOfRangeException( "Could not find property.", e );
                }
                else
                {
                    throw e;
                }
            }
            return pv.Value;
        }

        /// <summary>
        /// Retrieves a property value by the property index.
        /// Assumes the GUID component in the property key takes the sensor's type GUID.
        /// </summary>
        /// <param name="propIdx">A property index.</param>
        /// <returns>A property value.</returns>
        public object GetProperty( int propIdx )
        {
            PropertyKey propKey = new PropertyKey( SensorPropertyKeys.SENSOR_PROPERTY_COMMON_GUID, propIdx );
            return GetProperty( propKey );
        }

        /// <summary>
        /// Retrieves the values of multiple properties by property key.
        /// </summary>
        /// <param name="propKeys">An array of properties to retrieve.</param>
        /// <returns>A dictionary that contains the property keys and values.</returns>
        public IDictionary<PropertyKey, object> GetProperties( PropertyKey[ ] propKeys )
        {
            if( propKeys == null || propKeys.Length == 0 )
                throw new ArgumentNullException( "propKeys", "Property keys array must not be null or empty." );

            IPortableDeviceKeyCollection keyCollection = new PortableDeviceKeyCollection( );
            try
            {
                IPortableDeviceValues valuesCollection = null;

                for( int i = 0; i < propKeys.Length; i++ )
                {
                    PropertyKey propKey = propKeys[ i ];
                    keyCollection.Add( ref propKey );
                }

                Dictionary<PropertyKey, object> data = new Dictionary<PropertyKey, object>( );
                HRESULT hr = nativeISensor.GetProperties( keyCollection, out valuesCollection );
                if( valuesCollection != null )
                {
                    try
                    {

                        uint count = 0;
                        valuesCollection.GetCount( ref count );

                        for( uint i = 0; i < count; i++ )
                        {
                            PropertyKey propKey = new PropertyKey();
                            PropVariant propVal = new PropVariant();
                            valuesCollection.GetAt( i, ref propKey, ref propVal );

                            try
                            {
                                data.Add(propKey, propVal.Value);
                            }
                            finally
                            {
                                propVal.Clear( );
                            }
                        }
                    }
                    finally
                    {
                        Marshal.ReleaseComObject( valuesCollection );
                        valuesCollection = null;
                    }
                }

                return data;
            }
            finally
            {
                Marshal.ReleaseComObject( keyCollection );
                keyCollection = null;
            }
        }

        /// <summary>
        /// Returns a list of supported properties for the sensor.
        /// </summary>
        /// <returns>A strongly typed list of supported properties.</returns>
        public IList<PropertyKey> GetSupportedProperties( )
        {
            if( nativeISensor == null )
                throw new SensorPlatformException( "Sensor not initialized." );

            List<PropertyKey> list = new List<PropertyKey>( );
            IPortableDeviceKeyCollection collection;
            HRESULT hr = nativeISensor.GetSupportedDataFields( out collection );
            if( hr == HRESULT.S_OK )
            {
                try
                {
                    uint elements = 0;
                    collection.GetCount( out elements );
                    if( elements == 0 )
                        return null;

                    for( uint element = 0; element < elements; element++ )
                    {
                        PropertyKey key;
                        hr = collection.GetAt( element, out key );
                        if( hr == HRESULT.S_OK )
                        {
                            list.Add( key );
                        }
                    }
                }
                finally
                {
                    Marshal.ReleaseComObject( collection );
                    collection = null;
                }
            }
            return list;
        }


        /// <summary>
        /// Retrieves the values of multiple properties by their index.
        /// Assumes that the GUID component of the property keys is the sensor's type GUID.
        /// </summary>
        /// <param name="propIndices">The indexes of the properties to retrieve.</param>
        /// <returns>An array that contains the property values.</returns>
        /// <remarks>
        /// The returned array will contain null values for some properties if the values could not be retrieved.
        /// </remarks>
        public object[ ] GetProperties( params int[ ] propIndices )
        {
            if( propIndices == null || propIndices.Length == 0 )
                throw new ArgumentNullException( "propIndices", "Property keys array must not be null or empty." );

            IPortableDeviceKeyCollection keyCollection = new PortableDeviceKeyCollection( );
            try
            {
                IPortableDeviceValues valuesCollection;
                Dictionary<PropertyKey, int> propKeyToIdx = new Dictionary<PropertyKey, int>( );

                for( int i = 0; i < propIndices.Length; i++ )
                {
                    PropertyKey propKey = new PropertyKey( TypeId.Value, propIndices[ i ] );
                    keyCollection.Add( ref propKey );
                    propKeyToIdx.Add( propKey, i );
                }

                object[ ] data = new object[ propIndices.Length ];
                HRESULT hr = nativeISensor.GetProperties( keyCollection, out valuesCollection );
                if( hr == HRESULT.S_OK )
                {
                    try
                    {
                        if( valuesCollection == null )
                            return data;

                        uint count = 0;
                        valuesCollection.GetCount( ref count );

                        for( uint i = 0; i < count; i++ )
                        {
                            PropertyKey propKey = new PropertyKey();
                            PropVariant propVal = new PropVariant();
                            valuesCollection.GetAt( i, ref propKey, ref propVal );

                            try
                            {
                                int idx = propKeyToIdx[ propKey ];
                                data[idx] = propVal.Value;
                            }
                            finally
                            {
                                propVal.Clear( );
                            }
                        }
                    }
                    finally
                    {
                        Marshal.ReleaseComObject( valuesCollection );
                        valuesCollection = null;
                    }
                }
                return data;
            }
            finally
            {
                Marshal.ReleaseComObject( keyCollection );
            }
        }

        /// <summary>
        /// Sets the values of multiple properties.
        /// </summary>
        /// <param name="data">An array that contains the property keys and values.</param>
        /// <returns>A dictionary of the new values for the properties. Actual values may not match the requested values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        public IDictionary<PropertyKey, object> SetProperties( DataFieldInfo[ ] data )
        {
            if( data == null || data.Length == 0 )
                throw new ArgumentNullException( "data", "Data field array must not be null or empty." );

            IPortableDeviceValues pdv = new PortableDeviceValues( );

            for( int i = 0; i < data.Length; i++ )
            {
                PropertyKey propKey = data[ i ].Key;
                object value = data[ i ].Value;
                if( value == null )
                    throw new ArgumentNullException( "data", String.Format( "Data contains a null value at index {0}", i ) );

                try
                {
                    // PropVariant.FromObject will throw an ArgumentException if the value can 
                    // not be converted to an appropriate PropVariant.
                    PropVariant pv = PropVariant.FromObject( value );
                    pdv.SetValue( ref propKey, ref pv );
                }
                catch( ArgumentException )
                {
                    if( value is Guid )
                    {
                        Guid guid = (Guid)value;
                        pdv.SetGuidValue( ref propKey, ref guid );
                    }
                    else if( value is byte[ ] )
                    {
                        byte[ ] buffer = (byte[ ])value;
                        pdv.SetBufferValue( ref propKey, buffer, (uint)buffer.Length );
                    }
                    else
                    {
                        pdv.SetIUnknownValue( ref propKey, value );
                    }
                }
            }

            Dictionary<PropertyKey, object> results = new Dictionary<PropertyKey, object>( );
            IPortableDeviceValues pdv2 = null;
            HRESULT hr = nativeISensor.SetProperties( pdv, out pdv2 );
            if( hr == HRESULT.S_OK )
            {
                try
                {

                    uint count = 0;
                    pdv2.GetCount( ref count );

                    for( uint i = 0; i < count; i++ )
                    {
                        PropertyKey propKey = new PropertyKey();
                        PropVariant propVal = new PropVariant();
                        try
                        {
                            pdv2.GetAt( i, ref propKey, ref propVal );
                            results.Add(propKey, propVal.Value);
                        }
                        finally
                        {
                            propVal.Clear( );
                        }
                    }
                }
                finally
                {
                    Marshal.ReleaseComObject( pdv2 );
                    pdv2 = null;
                }
            }

            return results;
        }
        #endregion

        #region overridable methods
        /// <summary>
        /// Initializes the Sensor wrapper after it has been bound to the native ISensor interface
        /// and is ready for subsequent initialization.
        /// </summary>
        protected virtual void Initialize( )
        {
        }
        #endregion

        #region ISensorEvents Members

        void ISensorEvents.OnStateChanged( ISensor sensor, NativeSensorState state )
        {
            if( this.StateChanged != null )
            {
                StateChanged.Invoke( this, EventArgs.Empty );
            }
        }

        void ISensorEvents.OnDataUpdated( ISensor sensor, ISensorDataReport newData )
        {
            this.dataReport = SensorReport.FromNativeReport( this, newData );
            if( DataReportChanged != null )
            {
                DataReportChanged.Invoke( this, EventArgs.Empty );
            }
        }

        void ISensorEvents.OnEvent( ISensor sensor, Guid eventID, ISensorDataReport newData )
        {
        }

        void ISensorEvents.OnLeave( Guid sensorIdArgs )
        {
            SensorManager.OnSensorsChanged(sensorIdArgs, SensorAvailabilityChange.Removal);
        }

        #endregion

        #region Implementation
        private ISensor nativeISensor;
        internal ISensor internalObject
        {
            get
            {
                return nativeISensor;
            }
            set
            {
                nativeISensor = value;
                SetEventInterest( EventInterestTypes.StateChanged );
                nativeISensor.SetEventSink( this );
                Initialize( );
            }
        }

        /// <summary>
        /// Informs the sensor driver of interest in a specific type of event.
        /// </summary>
        /// <param name="eventType">The type of event of interest.</param>
        protected void SetEventInterest( Guid eventType )
        {
            if( this.nativeISensor == null )
                throw new SensorPlatformException( "Sensor not initialized" );

            Guid[ ] interestingEvents = GetInterestingEvents( );
            int interestCount = interestingEvents.Length;

            foreach( Guid g in interestingEvents )
            {
                if( g.CompareTo( eventType ) == 0 )
                    return;
            }

            Guid[ ] newEventInterest = new Guid[ interestCount + 1 ];
            interestingEvents.CopyTo( newEventInterest, 0 );
            newEventInterest[ interestCount ] = eventType;
            HRESULT hr = this.nativeISensor.SetEventInterest( newEventInterest, (uint)( interestCount + 1 ) );
            if( hr != HRESULT.S_OK )
            {
                Marshal.GetExceptionForHR( (int)hr );
            }
        }

        /// <summary>
        ///  Informs the sensor driver to clear a specific type of event.
        /// </summary>
        /// <param name="eventType">The type of event of interest.</param>
        protected void ClearEventInterest( Guid eventType )
        {
            if( this.nativeISensor == null )
                throw new SensorPlatformException( "Sensor not initialized" );

            if( IsEventInterestSet( eventType ) )
            {
                Guid[ ] interestingEvents = GetInterestingEvents( );
                int interestCount = interestingEvents.Length;

                Guid[ ] newEventInterest = new Guid[ interestCount - 1 ];

                int eventIndex = 0;
                foreach( Guid g in interestingEvents )
                {
                    if( g.CompareTo( eventType ) == 0 )
                    {
                        continue;
                    }
                    else
                    {
                        newEventInterest[ eventIndex ] = g;
                        eventIndex++;
                    }
                }

                this.nativeISensor.SetEventInterest( newEventInterest, (uint)( interestCount - 1 ) );
            }

        }
        /// <summary>
        /// Determines whether the sensor driver will file events for a particular type of event.
        /// </summary>
        /// <param name="eventType">The type of event, as a GUID.</param>
        /// <returns><b>true</b> if the sensor will report interest in the specified event.</returns>
        protected bool IsEventInterestSet( Guid eventType )
        {
            if( this.nativeISensor == null )
                throw new SensorPlatformException( "Sensor not initialized" );

            Guid[ ] interestingEvents = GetInterestingEvents( );

            foreach( Guid g in interestingEvents )
            {
                if( g.CompareTo( eventType ) == 0 )
                {
                    return true;
                }
            }
            return false;
        }

        private Guid[ ] GetInterestingEvents( )
        {
            IntPtr values;
            uint interestCount;
            this.nativeISensor.GetEventInterest( out values, out interestCount );
            Guid[ ] interestingEvents = new Guid[ interestCount ];
            for( int index = 0; index < interestCount; index++ )
            {
                interestingEvents[ index ] = (Guid)Marshal.PtrToStructure( values, typeof( Guid ) );
                values = IncrementIntPtr( values, Marshal.SizeOf( typeof( Guid ) ) );
            }
            return interestingEvents;
        }

        private IntPtr IncrementIntPtr( IntPtr source, int increment )
        {
            if( IntPtr.Size == 8 )
            {
                Int64 p = source.ToInt64( );
                p += increment;
                return new IntPtr( p );
            }
            else if( IntPtr.Size == 4 )
            {
                Int32 p = source.ToInt32( );
                p += increment;
                return new IntPtr( p );
            }
            else
            {
                throw new SensorPlatformException( "Expected pointer size" );
            }
        }

        #endregion
    }

    #region Helper types

    /// <summary>
    /// Defines a structure that contains the property ID (key) and value.
    /// </summary>
    public struct DataFieldInfo : IEquatable<DataFieldInfo>
    {
        private PropertyKey _propKey;
        private object _value;

        /// <summary>
        /// Initializes the structure.
        /// </summary>
        /// <param name="propKey">A property ID (key).</param>
        /// <param name="value">A property value. The type must be valid for the property ID.</param>
        public DataFieldInfo( PropertyKey propKey, object value )
        {
            _propKey = propKey;
            _value = value;
        }

        /// <summary>
        /// Gets the property's key.
        /// </summary>
        public PropertyKey Key
        {
            get { return _propKey; }
        }

        /// <summary>
        /// Gets the property's value.
        /// </summary>
        public object Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Returns the hash code for a particular DataFieldInfo structure.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode( )
        {
            int valHashCode = _value != null ? _value.GetHashCode( ) : 0;
            return _propKey.GetHashCode( ) ^ valHashCode;
        }

        /// <summary>
        /// Determines if this object and another object are equal.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><b>true</b> if this instance and another object are equal; otherwise <b>false</b>.</returns>
        public override bool Equals( object obj )
        {
            if( obj == null )
                return false;

            if( !( obj is DataFieldInfo ) )
                return false;

            DataFieldInfo other = (DataFieldInfo)obj;
            return _value.Equals( other._value ) && _propKey.Equals( other._propKey );
        }

        #region IEquatable<DataFieldInfo> Members

        /// <summary>
        /// Determines if this key and value pair and another key and value pair are equal.
        /// </summary>
        /// <param name="other">The item to compare.</param>
        /// <returns><b>true</b> if equal; otherwise <b>false</b>.</returns>
        public bool Equals( DataFieldInfo other )
        {
            return _value.Equals( other._value ) && _propKey.Equals( other._propKey );
        }

        #endregion

        /// <summary>
        /// DataFieldInfo == operator overload
        /// </summary>
        /// <param name="a">The first item to compare.</param>
        /// <param name="b">The second item to compare.</param>
        /// <returns><b>true</b> if equal; otherwise <b>false</b>.</returns>
        public static bool operator ==( DataFieldInfo a, DataFieldInfo b )
        {
            return a.Equals( b );
        }

        /// <summary>
        /// DataFieldInfo != operator overload
        /// </summary>
        /// <param name="a">The first item to compare.</param>
        /// <param name="b">The second item to comare.</param>
        /// <returns><b>true</b> if not equal; otherwise <b>false</b>.</returns>
        public static bool operator !=( DataFieldInfo a, DataFieldInfo b )
        {
            return !a.Equals( b );
        }
    }

    #endregion

}

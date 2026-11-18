using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

// Optional versioned trailer inside unused save-buffer space. Legacy fields and file sizes stay intact.
public static class GameSkillSaveExtension
{
    const int HeaderSize = 48;
    static readonly byte[] Magic = Encoding.ASCII.GetBytes( "SM_SKL01" );

    static byte[] Digest( byte[] bytes, int coreLength, byte[] payload )
    {
        using ( SHA256 sha = SHA256.Create() )
        {
            sha.TransformBlock( bytes, 0, coreLength, null, 0 );
            sha.TransformFinalBlock( payload, 0, payload.Length );
            return sha.Hash;
        }
    }

    public static void Write( byte[] bytes, int coreLength, GameUnitBase[] units )
    {
        using ( var stream = new MemoryStream() )
        using ( var writer = new BinaryWriter( stream ) )
        {
            int count = 0;
            foreach ( GameUnitBase unit in units )
                if ( unit.Skills.Length > GameDefine.MAX_SLOT ) count++;
            if ( count == 0 ) return; // Ordinary legacy saves remain byte-for-byte unchanged.
            if ( units.Length > byte.MaxValue ) throw new InvalidDataException( "Too many skill records." );
            writer.Write( (byte)count );
            for ( int i = 0; i < units.Length; i++ )
            {
                short[] skills = units[i].Skills;
                int extra = skills.Length - GameDefine.MAX_SLOT;
                if ( extra <= 0 ) continue;
                if ( extra > byte.MaxValue ) throw new InvalidDataException( "Too many extra skills." );
                writer.Write( (byte)i ); writer.Write( (byte)extra );
                for ( int slot = GameDefine.MAX_SLOT; slot < skills.Length; slot++ ) writer.Write( skills[slot] );
            }
            writer.Flush();
            byte[] payload = stream.ToArray();
            int header = bytes.Length - HeaderSize, start = header - payload.Length;
            if ( coreLength < 0 || start < coreLength ) throw new InvalidDataException( "Skill extension exceeds reserved save space." );
            Buffer.BlockCopy( payload, 0, bytes, start, payload.Length );
            Buffer.BlockCopy( Magic, 0, bytes, header, Magic.Length );
            Buffer.BlockCopy( BitConverter.GetBytes( coreLength ), 0, bytes, header + 8, 4 );
            Buffer.BlockCopy( BitConverter.GetBytes( payload.Length ), 0, bytes, header + 12, 4 );
            Buffer.BlockCopy( Digest( bytes, coreLength, payload ), 0, bytes, header + 16, 32 );
        }
    }

    // On an old save, clear any extras left in live objects by a previous load.
    // A corrupt extension falls back to its untouched legacy ten-slot core, with a visible log warning.
    public static bool Read( byte[] bytes, int minimumCoreLength, GameUnitBase[] units )
    {
        foreach ( GameUnitBase unit in units )
        {
            short[] legacy = new short[GameDefine.MAX_SLOT];
            for ( int i = 0; i < legacy.Length; i++ ) legacy[i] = i < unit.Skills.Length ? unit.Skills[i] : (short)GameDefine.INVALID_ID;
            unit.Skills = legacy;
        }
        int header = bytes.Length - HeaderSize;
        if ( header < 0 ) return false;
        for ( int i = 0; i < Magic.Length; i++ ) if ( bytes[header + i] != Magic[i] ) return false;
        try
        {
            int core = BitConverter.ToInt32( bytes, header + 8 ), length = BitConverter.ToInt32( bytes, header + 12 );
            if ( core < minimumCoreLength || core > header || length < 1 || length > header - core )
                throw new InvalidDataException( "Invalid skill extension bounds." );
            byte[] payload = new byte[length]; Buffer.BlockCopy( bytes, header - length, payload, 0, length );
            byte[] checksum = Digest( bytes, core, payload );
            for ( int i = 0; i < checksum.Length; i++ ) if ( checksum[i] != bytes[header + 16 + i] )
                throw new InvalidDataException( "Skill extension checksum mismatch." );
            var records = new Dictionary<int, short[]>();
            using ( var reader = new BinaryReader( new MemoryStream( payload ) ) )
            {
                int count = reader.ReadByte();
                if ( count > units.Length ) throw new InvalidDataException( "Too many skill records." );
                for ( int i = 0; i < count; i++ )
                {
                    int unit = reader.ReadByte(), extra = reader.ReadByte();
                    if ( unit >= units.Length || extra == 0 || records.ContainsKey( unit ) )
                        throw new InvalidDataException( "Invalid skill record." );
                    short[] skills = new short[GameDefine.MAX_SLOT + extra];
                    Array.Copy( units[unit].Skills, skills, GameDefine.MAX_SLOT );
                    for ( int j = GameDefine.MAX_SLOT; j < skills.Length; j++ )
                    {
                        skills[j] = reader.ReadInt16();
                        if ( skills[j] < 0 || Array.IndexOf( skills, skills[j], 0, j ) >= 0 )
                            throw new InvalidDataException( "Invalid extra skill." );
                    }
                    records.Add( unit, skills );
                }
                if ( reader.BaseStream.Position != reader.BaseStream.Length ) throw new InvalidDataException( "Trailing skill extension data." );
            }
            foreach ( var record in records ) units[record.Key].Skills = record.Value;
            return true;
        }
        catch ( Exception e ) when ( e is InvalidDataException || e is IOException || e is ArgumentException || e is OverflowException )
        {
            Debug.LogWarning( "额外技能存档校验失败，已保留原10槽技能：" + e.Message );
            return false;
        }
    }
}

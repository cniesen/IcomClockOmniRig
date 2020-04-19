/*
	IcomClockOmniRig
	Copyright (C) 2020  Claus Niesen

	This program is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
#include "OmniRigEventHandler.h"

OmniRigEventHandler::OmniRigEventHandler(){
}

STDMETHOD_(ULONG, AddRef)()
{
	InterlockedIncrement(&m_cRef);

	return m_cRef;
}

STDMETHOD_(ULONG, Release)()
{
	InterlockedDecrement(&m_cRef);

	if (m_cRef == 0)
	{
		delete this;
		return 0;
	}

	return m_cRef;
}

STDMETHOD(QueryInterface)(REFIID riid, void** ppvObject)
{
	if (riid == IID_IUnknown)
	{
		*ppvObject = (IUnknown*)this;
		AddRef();
		return S_OK;
	}

	if ((riid == IID_IDispatch) || (riid == __uuidof(device_event_interface)))
	{
		*ppvObject = (IDispatch*)this;
		AddRef();
		return S_OK;
	}

	return E_NOINTERFACE;
}

STDMETHOD(GetTypeInfoCount)(UINT* pctinfo)
{
	return E_NOTIMPL;
}

STDMETHOD(GetTypeInfo)(UINT itinfo, LCID lcid, ITypeInfo** pptinfo)
{
	return E_NOTIMPL;
}

STDMETHOD(GetIDsOfNames)(REFIID riid, LPOLESTR* rgszNames, UINT cNames,
	LCID lcid, DISPID* rgdispid)
{
	return E_NOTIMPL;
}

STDMETHOD(Invoke)(DISPID dispidMember, REFIID riid,
	LCID lcid, WORD wFlags, DISPPARAMS* pdispparams, VARIANT* pvarResult,
	EXCEPINFO* pexcepinfo, UINT* puArgErr)
{
	return (m_parent.*m_parent_on_invoke)(this, dispidMember, riid, lcid, wFlags, pdispparams, pvarResult, pexcepinfo, puArgErr);
}
*/